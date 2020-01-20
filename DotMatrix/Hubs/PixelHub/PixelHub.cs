using System;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;

namespace DotMatrix
{
	public class PixelHub : Hub
	{
		internal static async Task UpdateFrontEndPixel(PixelHubAddPixelRequest result)
		{
			var notificationHub = GlobalHost.ConnectionManager.GetHubContext<PixelHub>();
			if (notificationHub != null)
			{
				await notificationHub.Clients.All
					.OnPixelUpdate(new PixelHubPixelUpdate
					{
						X = result.X,
						Y = result.Y,
						Color = result.Color,
						Type = result.Type,
						NewPoints = result.NewPoints,

						Owner = result.UserName,
						Team = result.TeamName,
						IsApi = result.IsApi
					});
				await notificationHub.Clients.User(result.UserId.ToString())
					.OnPointsUpdate(new PixelHubPointsUpdate
					{
						Points = result.UserPoints
					});
			}
		}

		internal static async Task UpdateFrontEndPrize(PixelHubAddPrizeRequest request)
		{
			var notificationHub = GlobalHost.ConnectionManager.GetHubContext<PixelHub>();
			if (notificationHub != null)
			{
				await notificationHub.Clients.All
					.OnPrizeUpdate(new PixelHubPrizeUpdate
					{
						X = request.X,
						Y = request.Y,
						PrizeId = request.PrizeId,
						PrizeUser = request.PrizeUser,
						PrizeName = request.PrizeName,
						PrizePoints = request.PrizePoints,
					});

				await notificationHub.Clients.User(request.UserId.ToString())
					.OnNewPrize(new PixelHubNewPrizeUpdate
					{
						X = request.X,
						Y = request.Y,
						PrizeId = request.PrizeId,
						PrizeName = request.PrizeName,
						PrizeDescription = request.PrizeDescription,
						PrizePoints = request.PrizePoints,
					});

				await notificationHub.Clients.User(request.UserId.ToString())
					.OnPointsUpdate(new PixelHubPointsUpdate
					{
						Points = request.UserPoints
					});
			}
		}
	}
}