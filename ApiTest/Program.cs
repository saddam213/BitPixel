using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Base;
using DotMatrix.Common.DataContext;
using DotMatrix.Data.DataContext;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;

namespace ApiTest
{
	class Program
	{
		private static IDataContextFactory DataContextFactory = new DataContextFactory();
		static void Main(string[] args)
		{
			MainAsync().Wait();
			Console.ReadKey();
		}
		static async Task MainAsync()
		{

			var hub = new PixelHubClient();
			await Task.Delay(2000);
			await hub.InternalSendAward(new PixelHubAwardRequest
			{
				UserId = 1,
				AwardId = 22,
				AwardName = "hello",
				AwardPoints = 44,
				UserPoints = 45575
			});


			await Task.Delay(10000);
			await hub.InternalSendAward(new PixelHubAwardRequest
			{
				UserId = 1,
				AwardId = 22,
				AwardName = "hello",
				AwardPoints = 44,
				UserPoints = 45575
			});
			//using (var context = DataContextFactory.CreateContext())
			//{

			//	for (int i = 0; i < 600; i++)
			//	{
			//		context.Click.Add(new DotMatrix.Entity.Click
			//		{
			//			GameId = 1,
			//			Type = DotMatrix.Enums.PixelClickType.GetPixel,
			//			UserId = 1,
			//			Y = i,
			//			X = 4,
			//			Timestamp = DateTime.UtcNow
			//		});
			//	}

			//	await context.SaveChangesAsync();

			//}

		}

		public class PixelHubClient //: IQueueHubClient
		{
			public static IHubProxy Proxy { get; private set; }
			public static HubConnection Connection { get; private set; }
			protected bool _disconnecting = false;
			private static readonly string _authToken = ConfigurationManager.AppSettings["Signalr_AuthToken"];

			public PixelHubClient() : this(ConfigurationManager.AppSettings["PixelHub_Endpoint"]) { }
			public PixelHubClient(string endpoint)
			{
				Connection = new HubConnection(endpoint);
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

			public Task InternalSendAward(PixelHubAwardRequest request)
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

		public class PixelHubAwardRequest
		{
			public int UserId { get; set; }
			public int AwardId { get; set; }
			public string AwardName { get; set; }
			public int AwardPoints { get; set; }
			public int UserPoints { get; set; }
		}

		public class PixelRequest
		{
			public int X { get; set; }
			public int Y { get; set; }
			public byte R { get; set; }
			public byte G { get; set; }
			public byte B { get; set; }
			public string ApiKey { get; set; }
			public string Signature { get; set; }
			public string GetRequestBody()
			{
				return $"{nameof(X)}={X}&{nameof(Y)}={Y}&{nameof(R)}={R}&{nameof(G)}={G}&{nameof(B)}={B}";
			}
		}

		public class PixelResponse
		{
			public bool Success { get; set; }
			public string Message { get; set; }
		}

		private static async Task<PixelResponse> SendPixel(PixelRequest pixel)
		{
			string apiHost = "https://dotmatrix.chainstack.nz";
			string apiPublicKey = "1a4cfeed5aea423c942142e88d8f5033";
			string apiPrivateKey = "8thxiu2FuuFoRnfgDCL+R1EH/ZZ8V0eoPCPp8AYIndQ=";

			pixel.ApiKey = apiPublicKey;
			pixel.Signature = CreateSignature(apiPrivateKey, pixel.GetRequestBody());

			using (var client = new HttpClient())
			{
				var response = await client.PostAsJsonAsync($"{apiHost}/Api/AddPixel", pixel);
				return JsonConvert.DeserializeObject<PixelResponse>(await response.Content.ReadAsStringAsync());
			}
		}


		public static string CreateSignature(string secretKey, string dataToSign)
		{
			using (var hmacSha512 = new HMACSHA512(Encoding.ASCII.GetBytes(secretKey)))
			{
				return BitConverter.ToString(hmacSha512.ComputeHash(Encoding.ASCII.GetBytes(dataToSign))).Replace("-", string.Empty);
			}
		}
	}
}
