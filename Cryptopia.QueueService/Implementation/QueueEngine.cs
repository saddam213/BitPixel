using Cryptopia.QueueService.DataObjects;
using Cryptopia.Base.Logging;
using Cryptopia.Base.Queueing;
using DotMatrix.Common.DataContext;
using DotMatrix.Data.DataContext;
using DotMatrix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;


namespace Cryptopia.QueueService.Implementation
{
	public class QueueEngine
	{
		#region Fields
		public static readonly ProcessorQueue<IQueueItem, IQueueResponse> QueueProcessor = new ProcessorQueue<IQueueItem, IQueueResponse>(new QueueEngine().ProcessQueueItem);
		private readonly Log Log = LoggingManager.GetLog(typeof(QueueEngine));


		#endregion

		public QueueEngine()
		{
			DataContextFactory = new DataContextFactory();
		}

		public IDataContextFactory DataContextFactory { get; private set; }

		#region Processor

		public async Task<IQueueResponse> ProcessQueueItem(IQueueItem queueItem)
		{
			try
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
				{
					try
					{
						using (var context = DataContextFactory.CreateContext())
						{
							Log.Message(LogLevel.Info, $"ProcessQueueItem request received. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}");
							IQueueResponse queueResponse = null;
							if (queueItem is SubmitPixelRequest)
							{
								queueResponse = await ProcessSubmitPixelRequest(context, queueItem as SubmitPixelRequest).ConfigureAwait(false);
							}
							if (queueItem is SubmitPixelsRequest)
							{
								queueResponse = await ProcessSubmitPixelsRequest(context, queueItem as SubmitPixelsRequest).ConfigureAwait(false);
							}

							// commit the transaction
							Log.Message(LogLevel.Debug, "Committing database transaction...");
							transactionScope.Complete();
							Log.Message(LogLevel.Debug, "Successfully committed database transaction.");
							Log.Message(LogLevel.Info, $"ProcessQueueItem request complete. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}, Elapsed: {stopwatch.ElapsedMilliseconds}ms");
							if (queueResponse != null)
							{
								return queueResponse;
							}
						}
					}
					catch (QueueException tex)
					{
						Log.Message(LogLevel.Error, $"A QueueException was thrown. Error: {tex.Message}");
						return new ErrorResponse(tex.Message);
					}
					catch (Exception ex)
					{
						Log.Exception("An unknown exception occurred during request processing.", ex);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception("An unknown exception occurred during request processing.", ex);
			}
			return new ErrorResponse("An unknown error occurred. If problems persists please contact support");
		}

	#endregion


		private async Task<IQueueResponse> ProcessSubmitPixelRequest(IDataContext context, SubmitPixelRequest pixelRequest)
		{
			var pixel = pixelRequest.Pixel;
			var key = $"{pixel.X}-{pixel.Y}";
			var existingPixel = await context.Pixel.Where(x => x.PixelKey == key).FirstOrDefaultAsync();
			if (existingPixel == null)
			{
				existingPixel = new Pixel
				{
					X = pixel.X,
					Y = pixel.Y,
					PixelKey = key,
					Price = 0.000000005m,
					Color = "#FFFFFF",
					History = new List<PixelHistory>()
				};
				context.Pixel.Add(existingPixel);
			}

			existingPixel.UserId = pixelRequest.UserId;
			existingPixel.Color = pixel.Color;
			existingPixel.Price *= 2;
			existingPixel.LastUpdate = DateTime.UtcNow;
			existingPixel.History.Add(new PixelHistory
			{
				UserId = pixelRequest.UserId,
				Color = existingPixel.Color,
				Price = existingPixel.Price,
				Timestamp = existingPixel.LastUpdate
			});

			await context.SaveChangesAsync();

			return new SubmitPixelResponse();
		}

		private async Task<IQueueResponse> ProcessSubmitPixelsRequest(IDataContext context, SubmitPixelsRequest pixelsRequest)
		{
			foreach (var pixelRequest in pixelsRequest.Pixels)
			{
				var key = $"{pixelRequest.X}-{pixelRequest.Y}";
				var existingPixel = await context.Pixel.Where(x => x.PixelKey == key).FirstOrDefaultAsync();
				if (existingPixel == null)
				{
					existingPixel = new Pixel
					{
						X = pixelRequest.X,
						Y = pixelRequest.Y,
						PixelKey = key,
						Price = 0.000000005m,
						Color = "#FFFFFF",
						History = new List<PixelHistory>()
					};
					context.Pixel.Add(existingPixel);
				}

				existingPixel.UserId = pixelsRequest.UserId;
				existingPixel.Color = pixelRequest.Color;
				existingPixel.Price *= 2;
				existingPixel.LastUpdate = DateTime.UtcNow;
				existingPixel.History.Add(new PixelHistory
				{
					UserId = pixelsRequest.UserId,
					Color = existingPixel.Color,
					Price = existingPixel.Price,
					Timestamp = existingPixel.LastUpdate
				});
			}
			await context.SaveChangesAsync();

			return new SubmitPixelsResponse();
		}
	}
}