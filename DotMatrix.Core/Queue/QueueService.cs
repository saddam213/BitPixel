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

		public async Task<PixelResultModel> SubmitPixel(string userId, PixelModel model, bool isApi)
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
						R = model.R,
						G = model.G,
						B = model.B
					}
				});
				return new PixelResultModel
				{
					Success = response.Success,
					Message = response.Message,
					Balance = response.Balance
				};
			}
		}

		public Cryptopia.QueueService.QueueProcessorClient CreateService()
		{
			var client = new Cryptopia.QueueService.QueueProcessorClient();
#if !DEBUG
			client.ClientCredentials.Windows.ClientCredential.UserName = QueueServiceUsername;
			client.ClientCredentials.Windows.ClientCredential.Password = QueueServicePassword;
			client.ClientCredentials.Windows.ClientCredential.Domain = QueueServiceDomain;
#endif
			return client;
		}
	}
}
