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
		private decimal _pixelPrice = 1.00000000m;

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
			var user = await context.Users.Where(x => x.Id == pixelRequest.UserId).FirstOrDefaultAsync();
			if (user == null || _pixelPrice > user.Balance)
				return new ErrorResponse("Insufficient funds");

			var existingPixel = await context.Pixel.Where(x => x.PixelKey == key).FirstOrDefaultAsync();
			if (existingPixel == null)
			{
				existingPixel = new Pixel
				{
					X = pixel.X,
					Y = pixel.Y,
					PixelKey = key,
					R = 255,
					G = 255,
					B = 255,
					History = new List<PixelHistory>()
				};
				context.Pixel.Add(existingPixel);
			}

			user.Balance -= _pixelPrice;
			existingPixel.UserId = pixelRequest.UserId;
			existingPixel.R = pixel.R;
			existingPixel.G = pixel.G;
			existingPixel.B = pixel.B;
			existingPixel.LastUpdate = DateTime.UtcNow;
			existingPixel.History.Add(new PixelHistory
			{
				UserId = pixelRequest.UserId,
				R = existingPixel.R,
				G = existingPixel.G,
				B = existingPixel.B,
				Price = _pixelPrice,
				Timestamp = existingPixel.LastUpdate
			});

			await context.SaveChangesAsync();

			return new SubmitPixelResponse 
			{ 
			Success = true, 
			Balance = user.Balance,
			 Message = $"Successfully added new pixel at X:{pixel.X}, Y:{pixel.Y}"
			};
		}
	}
}