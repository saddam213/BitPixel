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
						RenderPixels(pixelGame, pixelGame.Pixels, "background", true);
					}
				}
				catch (Exception ex)
				{
					Log.Exception("", ex);
				}
			}
		}

		private void RenderPixels(Entity.Game game, IEnumerable<Entity.Pixel> pixels, string imageName, bool createQuadrant)
		{
			var outputDirectory = Path.Combine(_outputPath, $"{game.Id}");
			if (!Directory.Exists(outputDirectory))
				Directory.CreateDirectory(outputDirectory);

			using (var bitmapFixedSmall = new System.Drawing.Bitmap(game.Width, game.Height))
			using (var bitmapFixedLarge = new System.Drawing.Bitmap(game.Width * 10, game.Height * 10))
			using (var bitmapSmall = new System.Drawing.Bitmap(game.Width, game.Height))
			using (var bitmapLarge = new System.Drawing.Bitmap(game.Width * 10, game.Height * 10))
			{
				foreach (var pixel in pixels)
				{
					var px = pixel.X * 10;
					var py = pixel.Y * 10;
					var color = ColorTranslator.FromHtml(pixel.Color);
					bitmapSmall.SetPixel(pixel.X, pixel.Y, color);
					if(pixel.Type == Enums.PixelType.Fixed)
						bitmapFixedSmall.SetPixel(pixel.X, pixel.Y, color);

					for (int x = 0; x < 10; x++)
						for (int y = 0; y < 10; y++)
						{
							bitmapLarge.SetPixel(px + x, py + y, color);
							if (pixel.Type == Enums.PixelType.Fixed)
								bitmapFixedLarge.SetPixel(px + x, py + y, color);
						}

				}
				Log.Message(LogLevel.Info, "Processing complete, Saving Images..");
				bitmapFixedSmall.Save(Path.Combine(outputDirectory, $"{imageName}-fixed-small.png"), System.Drawing.Imaging.ImageFormat.Png);
				bitmapFixedLarge.Save(Path.Combine(outputDirectory, $"{imageName}-fixed-large.png"), System.Drawing.Imaging.ImageFormat.Png);
				bitmapSmall.Save(Path.Combine(outputDirectory, $"{imageName}-small.png"), System.Drawing.Imaging.ImageFormat.Png);
				bitmapLarge.Save(Path.Combine(outputDirectory, $"{imageName}-large.png"), System.Drawing.Imaging.ImageFormat.Png);

				if (createQuadrant)
				{
					var index = 0;
					var quadrantSmallSizeX = game.Width / 4;
					var quadrantSmallSizeY = game.Height / 4;
					var quadrantLargeSizeX = (game.Width * 10) / 4;
					var quadrantLargeSizeY = (game.Height * 10) / 4;
					for (int y = 0; y < 4; y++)
					{
						for (int x = 0; x < 4; x++)
						{
							var smallQuadrantRect = new Rectangle(x * quadrantSmallSizeX, y * quadrantSmallSizeY, quadrantSmallSizeX, quadrantSmallSizeY);
							using (var smallQuadrant = bitmapSmall.Clone(smallQuadrantRect, bitmapSmall.PixelFormat))
								smallQuadrant.Save(Path.Combine(outputDirectory, $"{imageName}-small-{index}.png"), System.Drawing.Imaging.ImageFormat.Png);

							var largeQuadranttRect = new Rectangle(x * quadrantLargeSizeX, y * quadrantLargeSizeY, quadrantLargeSizeX, quadrantLargeSizeY);
							using (var largeQuadrant = bitmapLarge.Clone(largeQuadranttRect, bitmapLarge.PixelFormat))
								largeQuadrant.Save(Path.Combine(outputDirectory, $"{imageName}-large-{index}.png"), System.Drawing.Imaging.ImageFormat.Png);

							index++;
						}
					}
				}

				using (var bitmapthumb = new System.Drawing.Bitmap(bitmapSmall, game.Width / 2, game.Height / 2))
				{
					bitmapthumb.Save(Path.Combine(outputDirectory, $"{imageName}-thumb.png"), System.Drawing.Imaging.ImageFormat.Png);
				}

				Log.Message(LogLevel.Info, $"{imageName} Images Saved.");
			}
		}

	}

	public class RenderPixelModel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
	}
}
