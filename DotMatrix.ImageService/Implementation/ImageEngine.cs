using Cryptopia.Base.Logging;
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

namespace DotMatrix.ImageService.Implementation
{
	public class ImageEngine
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(ImageEngine));
		private bool _isRunning = false;
		private CancellationToken _cancelToken;
		public DataContextFactory DataContextFactory { get; set; }
		public ImageEngine(CancellationToken cancelToken)
		{
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
					var path = @"D:\Repositories\DotMatrix\DotMatrix\Content\Images";
					var pixels = await context.Pixel.ToListAsync();

					using (var bitmapSmall = new System.Drawing.Bitmap(1000, 1000))
					using (var bitmapLarge = new System.Drawing.Bitmap(10000, 10000))
					{
						foreach (var pixel in pixels)
						{
							var px = pixel.X * 10;
							var py = pixel.Y * 10;
							var color = Color.FromArgb(pixel.R, pixel.G, pixel.B);
							bitmapSmall.SetPixel(pixel.X, pixel.Y, color);
							for (int x = 0; x < 10; x++)
								for (int y = 0; y < 10; y++)
									bitmapLarge.SetPixel(px + x, py + y, color);

						}
						Log.Message(LogLevel.Info, "Processing complete, Saving Images..");
						bitmapSmall.Save(Path.Combine(path, "background-small.png"), System.Drawing.Imaging.ImageFormat.Png);
						bitmapLarge.Save(Path.Combine(path, "background-large.png"), System.Drawing.Imaging.ImageFormat.Png);

						var index = 0;
						var quadrantSmallSize = 1000 / 4;
						var quadrantLargeSize = 10000 / 4;
						for (int y = 0; y < 4; y++)
						{
							for (int x = 0; x < 4; x++)
							{
								var smallQuadrantRect = new Rectangle(x * quadrantSmallSize, y * quadrantSmallSize, quadrantSmallSize, quadrantSmallSize);
								using (var smallQuadrant = bitmapSmall.Clone(smallQuadrantRect, bitmapSmall.PixelFormat))
									smallQuadrant.Save(Path.Combine(path, $"background-small-{index}.png"), System.Drawing.Imaging.ImageFormat.Png);

								var largeQuadranttRect = new Rectangle(x * quadrantLargeSize, y * quadrantLargeSize, quadrantLargeSize, quadrantLargeSize);
								using (var largeQuadrant = bitmapLarge.Clone(largeQuadranttRect, bitmapLarge.PixelFormat))
									largeQuadrant.Save(Path.Combine(path, $"background-large-{index}.png"), System.Drawing.Imaging.ImageFormat.Png);

								index++;
							}
						}
						Log.Message(LogLevel.Info, "Images Saved.");
					}
				}
				catch (Exception ex)
				{
					Log.Exception("", ex);
				}
			}
		}

	}
}
