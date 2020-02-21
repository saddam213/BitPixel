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
				await ProcessImages();
				await Task.Delay(TimeSpan.FromMinutes(1));
			}
		}

		private async Task ProcessImages()
		{
			Log.Message(LogLevel.Info, "Processing Images");
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
						RenderPixels(pixelGame, pixelGame.Pixels, "background");
					}
				}
				catch (Exception ex)
				{
					Log.Exception("", ex);
				}
			}
		}

		private void RenderPixels(Entity.Game game, ICollection<Entity.Pixel> pixels, string imageName)
		{
			Log.Message(LogLevel.Info, $"[RenderPixels] - Rendering game images, Game: {game.Name}, Pixels: {pixels.Count}");
			var start = DateTime.UtcNow;
			var outputDirectory = Path.Combine(_outputPath, $"{game.Id}");
			if (!Directory.Exists(outputDirectory))
				Directory.CreateDirectory(outputDirectory);

			using (var bitmapFixedSmall = new System.Drawing.Bitmap(game.Width, game.Height))
			using (var bitmapFixedLarge = new System.Drawing.Bitmap(game.Width * 5, game.Height * 5))
			using (var bitmapSmall = new System.Drawing.Bitmap(game.Width, game.Height))
			using (var bitmapLarge = new System.Drawing.Bitmap(game.Width * 5, game.Height * 5))
			{
				foreach (var pixel in pixels)
				{
					var px = pixel.X * 5;
					var py = pixel.Y * 5;
					var color = ColorTranslator.FromHtml(pixel.Color);
					bitmapSmall.SetPixel(pixel.X, pixel.Y, color);
					if(pixel.Type == Enums.PixelType.Fixed)
						bitmapFixedSmall.SetPixel(pixel.X, pixel.Y, color);

					for (int x = 0; x < 5; x++)
					{
						for (int y = 0; y < 5; y++)
						{
							bitmapLarge.SetPixel(px + x, py + y, color);
							if (pixel.Type == Enums.PixelType.Fixed)
								bitmapFixedLarge.SetPixel(px + x, py + y, color);
						}
					}
				}

				
				bitmapFixedSmall.Save(Path.Combine(outputDirectory, $"{imageName}-fixed-small.png"), System.Drawing.Imaging.ImageFormat.Png);
				bitmapFixedLarge.Save(Path.Combine(outputDirectory, $"{imageName}-fixed-large.png"), System.Drawing.Imaging.ImageFormat.Png);

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
	}
}
