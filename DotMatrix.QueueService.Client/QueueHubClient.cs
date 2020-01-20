using System;
using System.Configuration;
using System.Threading.Tasks;

using DotMatrix.QueueService.Common;

using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace DotMatrix.QueueService.Client
{
	public class QueueHubClient : IQueueHubClient
	{
		public static IHubProxy Proxy { get; private set; }
		public static HubConnection Connection { get; private set; }
		protected bool _disconnecting = false;

		public QueueHubClient() : this(ConfigurationManager.AppSettings["QueueService_Endpoint"]) { }
		public QueueHubClient(string endpoint)
		{
			Connection = new HubConnection(endpoint);
			Connection.Closed += async () => await TryReconnect();
			Proxy = Connection.CreateHubProxy("QueueHub");
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

		public Task<SubmitPixelResponse> SubmitPixel(SubmitPixelRequest request)
		{
			return SafeInvoke<SubmitPixelResponse>(nameof(SubmitPixel), request);
		}

		public Task<SubmitClickResponse> SubmitClick(SubmitClickRequest submitClickRequest)
		{
			return SafeInvoke<SubmitClickResponse>(nameof(SubmitClick), submitClickRequest);
		}

		private Task<T> SafeInvoke<T>(string methodName, object request)
		{
			if (Connection.State == ConnectionState.Connected)
				return Proxy.Invoke<T>(methodName, request);
			return Task.FromResult<T>(default(T));
		}
	}
}
