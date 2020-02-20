using System.Configuration;
using System.Threading.Tasks;

using DotMatrix.Base.Logging;
using DotMatrix.QueueService.Client;
using DotMatrix.QueueService.Common;
using DotMatrix.QueueService.Implementation;

using Microsoft.AspNet.SignalR;

namespace DotMatrix.QueueService.Hubs
{
	public class QueueHub : Hub, IQueueHubClient
	{
		private static readonly Log Log = LoggingManager.GetLog(typeof(QueueHub));
		private static readonly string _endPoint = ConfigurationManager.AppSettings["PixelHub_Endpoint"];
		private static readonly string _authToken = ConfigurationManager.AppSettings["Signalr_AuthToken"];
		private static PixelHubClient PixelHubClient = new PixelHubClient(_endPoint, _authToken);
		
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
			var queueResponse = await QueueEngine.QueueProcessor.QueueItem(request);
			if (!queueResponse.Success)
				return new SubmitPixelResponse { Success = queueResponse.Success, Message = queueResponse.Message };

			var response = queueResponse as SubmitPixelResponse;
			await Task.WhenAny
			(
				PixelHubClient.NotifyPixel(response.PixelNotification),
				PixelHubClient.NotifyPrize(response.PrizeNotification),
				PixelHubClient.NotifyPoints(response.PointsNotification)
			);
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
