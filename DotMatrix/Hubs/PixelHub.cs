using System.Threading.Tasks;

using DotMatrix.Base.Extensions;
using DotMatrix.Hubs;
using DotMatrix.QueueService.Common;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotMatrix
{
	public class PixelHub : Hub
	{
		[AuthorizeLocal]
		public Task NotifyPoints(PointsNotification notification)
		{
			return Task.WhenAll
			(
				Clients.User(notification.UserId)
					.OnUserNotifyPoints(new
					{
						notification.Points
					})
			);
		}

		[AuthorizeLocal]
		public Task NotifyAward(AwardNotification notification)
		{
			return Task.WhenAll
			(
				Clients.All
					.OnNotifyAward(new
					{
						AwardId = notification.AwardId,
						Level = notification.Level,
						Name = notification.Name,
						Points = notification.Points,
						Player = notification.UserName
					}),

				Clients.User(notification.UserId)
					.OnUserNotifyAward(new
					{
						AwardId = notification.AwardId,
						Level = notification.Level,
						Name = notification.Name,
						Points = notification.Points,
						Player = notification.UserName
					})
			);
		}


		[AuthorizeLocal]
		public Task NotifyPixel(PixelNotification notification)
		{
			return Task.WhenAll
			(
				Clients
					.Group(notification.GameId)
					.OnNotifyPixel(new
					{
						X = notification.X,
						Y = notification.Y,
						Color = notification.Color,
						Type = notification.Type,
						Points = notification.Points,

						Player = notification.UserName,
						Team = notification.TeamName
					})
			);
		}


		[AuthorizeLocal]
		public Task NotifyPrize(PrizeNotification notification)
		{
			return Task.WhenAll
			(
				Clients
					.Group(notification.GameId)
					.OnNotifyPrize(new
					{
						X = notification.X,
						Y = notification.Y,
						PrizeId = notification.PrizeId,
						Player = notification.UserName,
						Name = notification.Name,
						Points = notification.Points,
					}),

				Clients
					.User(notification.UserId)
					.OnUserNotifyPrize(new
					{
						X = notification.X,
						Y = notification.Y,
						GameId = notification.GameId,
						GameName = notification.GameName,
						PrizeId = notification.PrizeId,
						Name = notification.Name,
						Description = notification.Description,
						Points = notification.Points,
					})
			);
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



	}

	public static class HubExtensions
	{
		public static string GetUserHandle(this HubCallerContext context)
		{
			return context.User.Identity.IsAuthenticated
				? context.Request.GetHttpContext().User.Identity.Name
				: "Guest";
		}

		public static dynamic User(this IHubConnectionContext<dynamic> client, int userId)
		{
			return client.User(userId.ToString());
		}

		public static dynamic Group(this IHubConnectionContext<dynamic> client, int groupId)
		{
			return client.Group(groupId.ToString());
		}
	}
}