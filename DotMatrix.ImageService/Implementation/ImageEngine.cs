using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DotMatrix.Base.Logging;
using DotMatrix.Data.DataContext;
using DotMatrix.Enums;

namespace DotMatrix.ImageService.Implementation
{
	public class ImageEngine
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(ImageEngine));
		private bool _isRunning = false;
		private CancellationToken _cancelToken;
		private string _outputPath;

		public DataContextFactory DataContextFactory { get; set; }
		public ImageEngine(CancellationToken cancelToken, string outputPath)
		{
			_outputPath = outputPath;
			_cancelToken = cancelToken;
			DataContextFactory = new DataContextFactory();
		}

		public void Start()
		{
			if (_isRunning)
				return;

			_isRunning = true;
			Task.Factory.StartNew(async () => await ProcessLoop().ConfigureAwait(false), _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
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
			Log.Message(LogLevel.Info, "Processing Game Images");
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				try
				{
					var pixelGames = await context.Games
						.Include(x => x.Pixels)
						.Where(x => x.Status != Enums.GameStatus.Deleted && x.Status != Enums.GameStatus.Finished)
						.ToListAsync();
					foreach (var pixelGame in pixelGames)
					{
						var gamePixels = pixelGame.Pixels
							.Select(x => new RenderPixelModel
							{
								X = x.X,
								Y = x.Y,
								Type = x.Type,
								Color = x.Color
							});
						var userPixels = gamePixels.Where(x => x.Type == PixelType.User).ToList();
						var fixedPixels = gamePixels.Where(x => x.Type == PixelType.Fixed).ToList();
						RenderPixels(pixelGame, userPixels, "background");
						RenderPixels(pixelGame, fixedPixels, "background-fixed");
					}
					Log.Message(LogLevel.Info, $"Processing Game Images complete, Elapsed: {DateTime.UtcNow - start}ms");
				}
				catch (Exception ex)
				{
					Log.Exception("Exception Processing Game Images", ex);
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
								Type =  PixelType.User,
								Color = x.Color,
								Player = x.User.UserName
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


						RenderPixels(game, pixels, "Gallery\\pixels");
						RenderPixels(game, prizes, "Gallery\\prizes");

						foreach (var player in pixelHistory.GroupBy(x => x.Player))
						{
							var userPixels = player.OrderBy(x => x.Id).ToList();
							RenderPixels(game, userPixels, $"Gallery\\pixels-{player.Key}");
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

		private void RenderPixels(Entity.Game game, ICollection<RenderPixelModel> pixels, string imageName)
		{
			Log.Message(LogLevel.Info, $"[RenderPixels] - Rendering Pixels, GameId: {game.Id}, Pixels: {pixels.Count}, Image: {imageName}.png");
			var start = DateTime.UtcNow;
			var outputDirectory = Path.Combine(_outputPath, $"{game.Id}");
			if (!Directory.Exists(outputDirectory))
				Directory.CreateDirectory(outputDirectory);

			using (var bitmapSmall = new System.Drawing.Bitmap(game.Width, game.Height))
			using (var bitmapLarge = new System.Drawing.Bitmap(game.Width * 5, game.Height * 5))
			{
				foreach (var pixel in pixels)
				{
					var px = pixel.X * 5;
					var py = pixel.Y * 5;
					var color = ColorTranslator.FromHtml(pixel.Color);
					bitmapSmall.SetPixel(pixel.X, pixel.Y, color);


					for (int x = 0; x < 5; x++)
					{
						for (int y = 0; y < 5; y++)
						{
							bitmapLarge.SetPixel(px + x, py + y, color);
						}
					}
				}

				bitmapSmall.Save(Path.Combine(outputDirectory, $"{imageName}-small.png"), System.Drawing.Imaging.ImageFormat.Png);
				bitmapLarge.Save(Path.Combine(outputDirectory, $"{imageName}-large.png"), System.Drawing.Imaging.ImageFormat.Png);
				using (var bitmapthumb = new System.Drawing.Bitmap(bitmapSmall, game.Width / 2, game.Height / 2))
					bitmapthumb.Save(Path.Combine(outputDirectory, $"{imageName}-thumb.png"), System.Drawing.Imaging.ImageFormat.Png);
			}
			Log.Message(LogLevel.Info, $"Rendering game images complete, Elapsed: {DateTime.UtcNow - start}ms");
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
	}
}
