using DotMatrix.Api;
using DotMatrix.Common.Api;
using DotMatrix.Common.Pixel;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotMatrix.Controllers
{
	[ApiAuthentication]
	public class ApiPrivateController : ApiController
	{
		public IPixelWriter PixelWriter { get; set; }
		public async Task<ApiResponse> AddPixel(PixelModel model)
		{
			if (!model.IsValid())
				return new ApiResponse { Success = false, Message = "Invalid input data." };

			var result = await PixelWriter.AddOrUpdate(User.Identity.Name, model);
			if (!result)
				return new ApiResponse { Success = false, Message = "Failed to add pixel" };


			var notificationHub = GlobalHost.ConnectionManager.GetHubContext<PixelHub>();
			if (notificationHub != null)
			{
				await notificationHub.Clients.All.SendPixelData(model.X, model.Y, model.Color);
			}
			return new ApiResponse { Success = true, Message = $"Added new {model.Color} pixel at X:{model.X} Y:{model.Y}" };
		}
	}
}
