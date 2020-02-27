using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BitPixel.Base.Logging;
using BitPixel.Cache;
using BitPixel.Cache.Common;
using BitPixel.Data.DataContext;
using BitPixel.Enums;

namespace BitPixel.ImageService.Implementation
{
	public class ImageEngine
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(ImageEngine));
		private bool _isRunning = false;
		private CancellationToken _cancelToken;
		private string _outputPath;

		public IPixelCache PixelCache { get; set; }
		public DataContextFactory DataContextFactory { get; set; }
		public ImageEngine(CancellationToken cancelToken, string outputPath)
		{
			_outputPath = outputPath;
			_cancelToken = cancelToken;
			DataContextFactory = new DataContextFactory();
			PixelCache = new PixelCache("ImageSevice");
			PixelCache.Initialize().Wait();
		}

		public void Start()
		{
			if (_isRunning)
				return;

			_isRunning = true;
			Task.Factory.StartNew(ProcessLoop, _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
		}


		public void Stop()
		{
			_isRunning = false;
		}

		private async Task ProcessLoop()
		{
			while (_isRunning)
			{
				await ProcessLiveImages();
				await ProcessGalleryImages();
				await Task.Delay(TimeSpan.FromSeconds(30));
			}
		}

		private async Task ProcessLiveImages()
		{
			var start = DateTime.UtcNow;
			Log.Message(LogLevel.Info, "[ProcessLiveImages] - Processing Game Images");
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				try
				{
					var pixelGames = await context.Games
						.Where(x => x.Status != Enums.GameStatus.Deleted && x.Status != Enums.GameStatus.Finished)
						.ToListAsync();
					foreach (var pixelGame in pixelGames)
					{
						var gamePixels = (await PixelCache.GetPixels(pixelGame.Id))
							.Select(x => new RenderPixelModel
							{
								X = x.X,
								Y = x.Y,
								Type = x.Type,
								Color = x.Color
							}).ToList();

						var fixedPixels = gamePixels.Where(x => x.Type == PixelType.Fixed).ToList();
						RenderPixels(pixelGame, gamePixels, "background");
						RenderPixels(pixelGame, fixedPixels, "background-fixed", true);
					}
					Log.Message(LogLevel.Info, $"[ProcessLiveImages] - Processing Game Images complete, Elapsed: {DateTime.UtcNow - start}ms");
				}
				catch (Exception ex)
				{
					Log.Exception("[ProcessLiveImages] - Exception Processing Game Images", ex);
				}
			}
		}

		private async Task ProcessGalleryImages()
		{
			var start = DateTime.UtcNow;
			Log.Message(LogLevel.Info, "Processing Gallery Images");
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				try
				{
					var pixelGames = await context.Games
						.Where(x => x.Status == Enums.GameStatus.Finished)
						.ToListAsync();
					foreach (var game in pixelGames)
					{
						var outputDirectory = Path.Combine(_outputPath, $"{game.Id}\\Gallery");
						if (Directory.Exists(outputDirectory))
							continue;

						Log.Message(LogLevel.Info, $"Generating Gallery Images... GameId: {game}");
						Directory.CreateDirectory(outputDirectory);

						var pixels = await context.Pixel
							.Where(x => x.GameId == game.Id)
							.Select(x => new RenderPixelModel
							{
								X = x.X,
								Y = x.Y,
								Type = x.Type,
								Color = x.Color
							}).ToListAsync();

						var pixelHistory = await context.PixelHistory
							.Where(x => x.GameId == game.Id)
							.Select(x => new RenderPixelModel
							{
								Id = x.Id,
								X = x.Pixel.X,
								Y = x.Pixel.Y,
								Type = PixelType.User,
								Color = x.Color,
								Player = x.User.UserName,
								Points = x.Points,
								Team = x.Team != null ? x.Team.Name : string.Empty
							}).ToListAsync();

						var prizes = await context.Prize
							.Where(x => x.GameId == game.Id && x.IsClaimed)
							.Select(x => new RenderPixelModel
							{
								X = x.X,
								Y = x.Y,
								Type = PixelType.User,
								Color = "#FFA500"
							}).ToListAsync();

						// Final background images
						var allUserPixels = pixels.Where(x => x.Type == PixelType.User).ToList();
						var allFixedPixels = pixels.Where(x => x.Type == PixelType.Fixed).ToList();
						RenderPixels(game, allUserPixels, "background");
						RenderPixels(game, allFixedPixels, "background-fixed");

						// Render gallrey images
						RenderPixels(game, pixels, "Gallery\\pixels");
						RenderPixels(game, prizes, "Gallery\\prizes");

						// Player Pixels
						foreach (var player in pixelHistory.GroupBy(x => x.Player))
						{
							var userPixels = player.OrderBy(x => x.Id).ToList();
							RenderPixels(game, userPixels, $"Gallery\\pixels-{player.Key}");
						}

						// Team Pixels
						if (game.Type == GameType.TeamBattle)
						{
							var teamPixelGroups = pixelHistory
								.GroupBy(x => x.Team)
								.OrderByDescending(x => x.Sum(p => p.Points))
								.ToList();
							foreach (var teamPixelGroup in teamPixelGroups)
							{
								var teamPixels = teamPixelGroup.OrderBy(x => x.Id).ToList();
								RenderPixels(game, teamPixels, $"Gallery\\pixels-team-{teamPixelGroup.Key}");
							}
						}
						Log.Message(LogLevel.Info, $"Generating Gallery Images complete.");
					}
					Log.Message(LogLevel.Info, $"Processing Gallery Images complete, Elapsed: {DateTime.UtcNow - start}ms");
				}
				catch (Exception ex)
				{
					Log.Exception("Exception Processing Gallery Images", ex);
				}
			}
		}

		private void RenderPixels(Entity.Game game, ICollection<RenderPixelModel> pixels, string imageName, bool skipIfExists = false)
		{
			var start = DateTime.UtcNow;
			var outputDirectory = Path.Combine(_outputPath, $"{game.Id}");
			if (!Directory.Exists(outputDirectory))
				Directory.CreateDirectory(outputDirectory);

			var filename = Path.Combine(outputDirectory, $"{imageName}.png");
			var filenameThumb = Path.Combine(outputDirectory, $"{imageName}-thumb.png");
			if (skipIfExists && File.Exists(filename))
				return;

			Log.Message(LogLevel.Info, $"[RenderPixels] - Rendering Pixels, GameId: {game.Id}, Pixels: {pixels.Count}, Image: {imageName}.png");
			using (var bitmap = new Bitmap(game.Width, game.Height))
			{
				foreach (var pixel in pixels)
				{
					bitmap.SetPixel(pixel.X, pixel.Y, ColorTranslator.FromHtml(pixel.Color));
				}

				bitmap.Save(filename, ImageFormat.Png);
				using (var bitmapthumb = new Bitmap(bitmap, game.Width / 2, game.Height / 2))
				{
					bitmapthumb.Save(filenameThumb, ImageFormat.Png);
				}
			}
			Log.Message(LogLevel.Info, $"[RenderPixels] - Rendering pixels complete, Elapsed: {DateTime.UtcNow - start}ms");
		}
	}

	public class RenderPixelModel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public string Player { get; set; }
		public int Id { get; set; }
		public string Team { get; set; }
		public int Points { get; set; }
	}
}
