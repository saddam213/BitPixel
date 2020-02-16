using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace DotMatrix.AwardService.Implementation
{
	public class NotificationProxy
	{
		public static IHubProxy Proxy { get; private set; }
		public static HubConnection Connection { get; private set; }
		protected bool _disconnecting = false;
		private static readonly string _endPoint = ConfigurationManager.AppSettings["PixelHub_Endpoint"];
		private static readonly string _authToken = ConfigurationManager.AppSettings["Signalr_AuthToken"];

		public NotificationProxy()
		{
			Connection = new HubConnection(_endPoint);
			Connection.Headers.Add("auth", _authToken);
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

		public Task InternalSendAward(AddAwardResult request)
		{
			return SafeInvoke(nameof(InternalSendAward), request);
		}

		private Task SafeInvoke(string methodName, object request)
		{
			if (Connection.State == ConnectionState.Connected)
				return Proxy.Invoke(methodName, request);
			return Task.FromResult(0);
		}
	}

	//public class PixelHubAwardRequest
	//{
	//	public int UserId { get; set; }
	//	public int AwardId { get; set; }
	//	public string AwardName { get; set; }
	//	public int AwardPoints { get; set; }
	//	public int UserPoints { get; set; }
	//}
}
