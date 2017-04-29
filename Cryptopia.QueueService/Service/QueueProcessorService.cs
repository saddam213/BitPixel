using Cryptopia.QueueService.DataObjects;
using Cryptopia.QueueService.Implementation;
using Cryptopia.Base.Logging;
using System;
using System.ServiceModel;
using System.Threading.Tasks;


namespace Cryptopia.QueueService.Service
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.PerCall)]
	public class QueueProcessorService : IQueueProcessor
	{
		private static readonly Log Log = LoggingManager.GetLog(typeof(QueueProcessorService));
		

		public async Task<SubmitPixelResponse> SubmitPixel(SubmitPixelRequest request)
		{
			try
			{
				Log.Message(LogLevel.Verbose, "SubmitPixelRequest received.");
				var response = await QueueEngine.QueueProcessor.QueueItem(request).ConfigureAwait(false);
				if (!response.Success)
					return new SubmitPixelResponse { Success = false, Message = response.Message };

				return response as SubmitPixelResponse;
			}
			catch (Exception ex)
			{
				Log.Exception("An unknown exception occurred processing SubmitPixelRequest.", ex);
				return new SubmitPixelResponse { Success = false, Message = "An unknown error occurred processing QueueItem." };
			}
		}

		public async Task<SubmitPixelsResponse> SubmitPixels(SubmitPixelsRequest request)
		{
			try
			{
				Log.Message(LogLevel.Verbose, "SubmitPixelsRequest received.");
				var response = await QueueEngine.QueueProcessor.QueueItem(request).ConfigureAwait(false);
				if (!response.Success)
					return new SubmitPixelsResponse { Success = false, Message = response.Message };

				return response as SubmitPixelsResponse;
			}
			catch (Exception ex)
			{
				Log.Exception("An unknown exception occurred processing SubmitPixelsRequest.", ex);
				return new SubmitPixelsResponse { Success = false, Message = "An unknown error occurred processing QueueItem." };
			}
		}
	}
}