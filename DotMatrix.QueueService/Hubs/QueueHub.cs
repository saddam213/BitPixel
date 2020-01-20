using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			return await QueueEngine.PixelQueueProcessor.QueueItem(request);
		}

		public async Task<SubmitClickResponse> SubmitClick(SubmitClickRequest request)
		{
			return await QueueEngine.ClickQueueProcessor.QueueItem(request);
		}
	}
}
