using DotMatrix.Base.Logging;
using DotMatrix.Data.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
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
				await ProcessImages();
				await Task.Delay(TimeSpan.FromMinutes(1));
			}
		}

		private async Task ProcessImages()
		{
			Log.Message(LogLevel.Info, "Processing Images");
			using (var context = DataContextFactory.CreateContext())
			{
				try
				{
					var pixels = await context.Pixel
						.Select(x => new RenderPixelModel
						{
							X = x.X,
							Y = x.Y,
							Color = x.Color
						}).ToListAsync();
					RenderPixels(pixels, "background", true);
				}
				catch (Exception ex)
				{
					Log.Exception("", ex);
				}
			}
		}

		private void RenderPixels(IEnumerable<RenderPixelModel> pixels, string imageName, bool createQuadrant)
		{

			using (var bitmapSmall = new System.Drawing.Bitmap(Constant.Width, Constant.Height))
			using (var bitmapLarge = new System.Drawing.Bitmap(Constant.Width * 10, Constant.Height * 10))
			{
				foreach (var pixel in pixels)
				{
					var px = pixel.X * 10;
					var py = pixel.Y * 10;
					var color = ColorTranslator.FromHtml(pixel.Color);
					bitmapSmall.SetPixel(pixel.X, pixel.Y, color);
					for (int x = 0; x < 10; x++)
						for (int y = 0; y < 10; y++)
							bitmapLarge.SetPixel(px + x, py + y, color);

				}
				Log.Message(LogLevel.Info, "Processing complete, Saving Images..");
				bitmapSmall.Save(Path.Combine(_outputPath, $"{imageName}-small.png"), System.Drawing.Imaging.ImageFormat.Png);
				bitmapLarge.Save(Path.Combine(_outputPath, $"{imageName}-large.png"), System.Drawing.Imaging.ImageFormat.Png);

				if (createQuadrant)
				{
					var index = 0;
					var quadrantSmallSizeX = Constant.Width / 4;
					var quadrantSmallSizeY = Constant.Height / 4;
					var quadrantLargeSizeX = (Constant.Width * 10) / 4;
					var quadrantLargeSizeY = (Constant.Height * 10) / 4;
					for (int y = 0; y < 4; y++)
					{
						for (int x = 0; x < 4; x++)
						{
							var smallQuadrantRect = new Rectangle(x * quadrantSmallSizeX, y * quadrantSmallSizeY, quadrantSmallSizeX, quadrantSmallSizeY);
							using (var smallQuadrant = bitmapSmall.Clone(smallQuadrantRect, bitmapSmall.PixelFormat))
								smallQuadrant.Save(Path.Combine(_outputPath, $"{imageName}-small-{index}.png"), System.Drawing.Imaging.ImageFormat.Png);

							var largeQuadranttRect = new Rectangle(x * quadrantLargeSizeX, y * quadrantLargeSizeY, quadrantLargeSizeX, quadrantLargeSizeY);
							using (var largeQuadrant = bitmapLarge.Clone(largeQuadranttRect, bitmapLarge.PixelFormat))
								largeQuadrant.Save(Path.Combine(_outputPath, $"{imageName}-large-{index}.png"), System.Drawing.Imaging.ImageFormat.Png);

							index++;
						}
					}
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
