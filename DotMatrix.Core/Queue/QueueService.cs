using DotMatrix.Common.Pixel;
using DotMatrix.Common.Queue;
using DotMatrix.Core.Cryptopia.QueueService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Core.Queue
{
	public class QueueService : IQueueService
	{
		private static readonly string QueueServiceUsername = ConfigurationManager.AppSettings["QueueServiceUsername"];
		private static readonly string QueueServicePassword = ConfigurationManager.AppSettings["QueueServicePassword"];
		private static readonly string QueueServiceDomain = ConfigurationManager.AppSettings["QueueServiceDomain"];

		public async Task<bool> SubmitPixel(string userId, PixelModel model, bool isApi)
		{
			try
			{
				using (var queueService = CreateService())
				{
					var response = await queueService.SubmitPixelAsync(new SubmitPixelRequest
					{
						IsApi = isApi,
						UserId = userId,
						Pixel = new PixelItem
						{
							X = model.X,
							Y = model.Y,
							Color = model.Color
						}
					});
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SubmitPixels(string userId, List<PixelModel> model, bool isApi)
		{
			try
			{
				using (var queueService = CreateService())
				{
					var response = await queueService.SubmitPixelsAsync(new SubmitPixelsRequest
					{
						IsApi = isApi,
						UserId = userId,
						Pixels = model.Select(x => new PixelItem
						{
							X = x.X,
							Y = x.Y,
							Color = x.Color
						}).ToList()
					});
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public QueueProcessorClient CreateService()
		{
			var client = new QueueProcessorClient();
#if !DEBUG
			client.ClientCredentials.Windows.ClientCredential.UserName = QueueServiceUsername;
			client.ClientCredentials.Windows.ClientCredential.Password = QueueServicePassword;
			client.ClientCredentials.Windows.ClientCredential.Domain = QueueServiceDomain;
#endif
			return client;
		}
	}
}
