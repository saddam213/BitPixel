using System.Threading.Tasks;

using DotMatrix.Base.Logging;
using DotMatrix.QueueService.Common;
using DotMatrix.QueueService.Implementation;

using Microsoft.AspNet.SignalR;

namespace DotMatrix.QueueService.Hubs
{
	public class QueueHub : Hub, IQueueHubClient
	{
		private static readonly Log Log = LoggingManager.GetLog(typeof(QueueHub));

		public override Task OnConnected()
		{
			Log.Message(LogLevel.Debug, $"{Context.ConnectionId}");
			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			Log.Message(LogLevel.Debug, $"{Context.ConnectionId}");
			return base.OnDisconnected(stopCalled);
		}

		public async Task<SubmitPixelResponse> SubmitPixel(SubmitPixelRequest request)
		{
			var response = await QueueEngine.QueueProcessor.QueueItem(request);
			if (!response.Success)
				return new SubmitPixelResponse { Success = response.Success, Message = response.Message };

			return response as SubmitPixelResponse;
		}

		public async Task<SubmitClickResponse> SubmitClick(SubmitClickRequest request)
		{
			var response = await QueueEngine.QueueProcessor.QueueItem(request);
			if (!response.Success)
				return new SubmitClickResponse { Success = response.Success, Message = response.Message };

			return response as SubmitClickResponse;
		}
	}
}
