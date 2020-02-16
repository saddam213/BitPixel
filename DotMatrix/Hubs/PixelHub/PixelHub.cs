using System;
using System.Threading.Tasks;
using DotMatrix.Base.Extensions;
using DotMatrix.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotMatrix
{
	public class PixelHub : Hub
	{

		[AuthorizeLocal]
		public async Task InternalSendAward(PixelHubAwardRequest award)
		{
			await Clients.User(award.UserId.ToString())
				.OnNewAward(award);

			await Clients
				.User(award.UserId.ToString())
				.OnPointsUpdate(new PixelHubPointsUpdate
				{
					Points = award.UserPoints
				});
		}

		public Task JoinGame(int gameId)
		{
			return Groups.Add(Context.ConnectionId, gameId.ToString());
		}

		[Authorize]
		public Task SendChatMessage(string message)
		{
			return Clients.All.OnChatMessage(new
			{
				Sender = Context.GetUserHandle(),
				Message = message.Truncate(240)
			});
		}

		internal static async Task UpdateFrontEndPixel(PixelHubAddPixelRequest result)
		{
			var notificationHub = GlobalHost.ConnectionManager.GetHubContext<PixelHub>();
			if (notificationHub != null)
			{
				await notificationHub.Clients
					.Group(result.GameId.ToString())
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
				await notificationHub.Clients
					.User(result.UserId.ToString())
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
				await notificationHub.Clients
					.Group(request.GameId.ToString())
					.OnPrizeUpdate(new PixelHubPrizeUpdate
					{
						X = request.X,
						Y = request.Y,
						PrizeId = request.PrizeId,
						PrizeUser = request.PrizeUser,
						PrizeName = request.PrizeName,
						PrizePoints = request.PrizePoints,
					});

				await notificationHub.Clients
					.User(request.UserId.ToString())
					.OnNewPrize(new PixelHubNewPrizeUpdate
					{
						X = request.X,
						Y = request.Y,
						GameId = request.GameId,
						GameName = request.GameName,
						PrizeId = request.PrizeId,
						PrizeName = request.PrizeName,
						PrizeDescription = request.PrizeDescription,
						PrizePoints = request.PrizePoints,
					});

				await notificationHub.Clients
					.User(request.UserId.ToString())
					.OnPointsUpdate(new PixelHubPointsUpdate
					{
						Points = request.UserPoints
					});
			}
		}
	}

	public static class HubExtensions
	{
		public static string GetUserHandle(this HubCallerContext context)
		{
			return context.User.Identity.IsAuthenticated
				? context.Request.GetHttpContext().User.Identity.Name
				: "Guest";
		}
	}
}