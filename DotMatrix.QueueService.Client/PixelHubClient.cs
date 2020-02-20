using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.QueueService.Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace DotMatrix.QueueService.Client
{
	public class PixelHubClient
	{
		public static IHubProxy Proxy { get; private set; }
		public static HubConnection Connection { get; private set; }
		protected bool _disconnecting = false;

		public PixelHubClient(string endPoint, string authToken)
		{
			Connection = new HubConnection(endPoint);
			Connection.Headers.Add("auth", authToken);
			Connection.Closed += async () => await TryReconnect();
			Proxy = Connection.CreateHubProxy("PixelHub");
			Start();
		}

		public ConnectionState ConnectionState
		{
			get { return Connection.State; }
		}

		public Task Start()
		{
			_disconnecting = false;
			return TryConnect();
		}

		public Task Stop()
		{
			_disconnecting = true;
			Connection.Stop();
			return Task.FromResult(true);
		}

		private async Task TryReconnect()
		{
			if (!_disconnecting)
			{
				await Task.Delay(2000);
				await TryConnect();
			}
		}

		private async Task TryConnect()
		{
			try
			{
				await Connection.Start(new WebSocketTransport()
				{
					ReconnectDelay = TimeSpan.FromSeconds(1)
				});
			}
			catch (Exception ex)
			{
			}
		}

		public Task NotifyPoints(PointsNotification notification)
		{
			return SafeInvoke(nameof(NotifyPoints), notification);
		}

		public Task NotifyPixel(PixelNotification notification)
		{
			return SafeInvoke(nameof(NotifyPixel), notification);
		}

		public Task NotifyPrize(PrizeNotification notification)
		{
			return SafeInvoke(nameof(NotifyPrize), notification);
		}

		public Task NotifyAward(AwardNotification notification)
		{
			return SafeInvoke(nameof(NotifyAward), notification);
		}

		private Task SafeInvoke(string methodName, object request)
		{
			if (request == null)
				return Task.FromResult(0);

			if (Connection.State == ConnectionState.Connected)
				return Proxy.Invoke(methodName, request);
			return Task.FromResult(0);
		}
	}
}