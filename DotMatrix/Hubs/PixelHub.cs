using System;
using System.Threading.Tasks;

using DotMatrix.Base.Extensions;
using DotMatrix.Cache.Common;
using DotMatrix.Common.Game;
using DotMatrix.Common.Pixel;
using DotMatrix.Enums;
using DotMatrix.Hubs;
using DotMatrix.QueueService.Common;
using DotMatrix.Helpers;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace DotMatrix
{
	public class PixelHub : Hub
	{
		public IGameCache GameCache { get; set; }
		public IPixelCache PixelCache { get; set; }

		public IGameReader GameReader { get; set; }
		public IPixelWriter PixelWriter { get; set; }
		public IPixelReader PixelReader { get; set; }
		public IThrottleCache ThrottleCache { get; set; }

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

						Player = notification.UserName
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

		public async Task JoinGame(int gameId)
		{
			var game = await GameCache.GetGame(gameId);
			if (game != null)
				await Groups.Add(Context.ConnectionId, game.Id.ToString());
		}

		public async Task<PixelResult> GameStats(int gameId)
		{
			var stats = await GameReader.GetStats(gameId);
			if (stats == null)
				return new PixelResult(false, "Game stats not found");

			return new PixelResult(stats);
		}



		[Authorize]
		public async Task<PixelResult> AddPixel(AddPixelRequest model)
		{
			var game = await GameCache.GetGame(model.GameId);
			if (game == null)
				return new PixelResult(false, "Game not found");

			if (game.Status != GameStatus.Started)
				return new PixelResult(false, "Game is not currently active");

			var pixel = await PixelCache.GetPixel(game.Id, model.X, model.Y);
			if (pixel != null)
			{
				if (pixel.Type == PixelType.Fixed)
					return new PixelResult(false, "Cannot overwrite gameboard pixel");

				if (pixel.Points > model.MaxPoints)
					return new PixelResult(false, "Pixel points are greater than Spend Limit");
			}

			var userId = Context.GetUserId();
			var rateLimitResult = await CheckRateLimits(userId, game);
			if (!string.IsNullOrEmpty(rateLimitResult))
				return new PixelResult(false, rateLimitResult);

			var result = await PixelWriter.AddPixel(userId, model);
			if (!result.Success)
				return new PixelResult(false, result.Message);

			return new PixelResult(result);
		}

		public async Task<PixelResult> GetPixel(GetPixelRequest model)
		{
			var game = await GameCache.GetGame(model.GameId);
			if (game == null)
				return new PixelResult(false, "Game not found");

			if (Context.IsAuthenticated())
			{
				if (game.Status == GameStatus.Started)
				{
					var userId = Context.GetUserId();
					var rateLimitResult = await CheckRateLimits(userId, game);
					if (!string.IsNullOrEmpty(rateLimitResult))
						return new PixelResult(false, rateLimitResult);

					var clickResult = await PixelWriter.AddClick(userId, new AddClickRequest(model.GameId, model.X, model.Y));
					if (!clickResult.Success)
						return new PixelResult(false, clickResult.Message);
				}
			}

			var result = await PixelReader.GetPixel(model.GameId, model.X, model.Y);
			if (result == null)
				return new PixelResult(false, "Pixel not found.");

			return new PixelResult(result);
		}



		private async Task<string> CheckRateLimits(int userId, GameCacheItem game)
		{
			if (!await ThrottleCache.ShouldExcecute(userId, $"GameClicks:{game.Id}", TimeSpan.FromSeconds(1), game.ClicksPerSecond))
				return $"Maximum {game.ClicksPerSecond} clicks per second";

			if (!await ThrottleCache.ShouldExcecute(userId, "UserClicks", TimeSpan.FromHours(24), Constant.ClicksPerDay))
				return $"Maximum {Constant.ClicksPerDay} clicks per day";

			if (!await ThrottleCache.ShouldExcecute(Context.GetIPAddress(), "IPAddressClicks", TimeSpan.FromHours(24), Constant.ClicksPerDay))
				return $"Maximum {Constant.ClicksPerDay} clicks per day (ip)";

			return string.Empty;
		}



		//Chat-----------------------------

		[Authorize]
		public async Task SendChatMessage(string message)
		{
			var chatMessage = new ChatMessage(Context.GetUserName(), message.Truncate(240));
			AddChatHistory(chatMessage);
			await Clients.All.OnChatMessage(chatMessage);
		}

		public Task<IEnumerable<ChatMessage>> GetChatMessages()
		{
			return Task.FromResult<IEnumerable<ChatMessage>>(ChatHistory);
		}

		private static ConcurrentQueue<ChatMessage> ChatHistory = new ConcurrentQueue<ChatMessage>();
		private static void AddChatHistory(ChatMessage message)
		{
			ChatHistory.Enqueue(message);
			if (ChatHistory.Count > 50)
			{
				ChatHistory.TryDequeue(out _);
			}
		}
	}

	public class ChatMessage
	{
		public ChatMessage() { }
		public ChatMessage(string sender, string message)
		{
			Sender = sender;
			Message = message;
			Timestamp = DateTime.UtcNow.ToUnixMs();
		}
		public string Sender { get; set; }
		public string Message { get; set; }
		public long Timestamp { get; set; }
	}

	public class PixelResult
	{
		public PixelResult() { }
		public PixelResult(bool success, string message)
		{
			Success = success;
			Message = message;
		}
		public PixelResult(object data)
		{
			Success = true;
			Data = data;
		}
		public bool Success { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }
	}



	public static class HubExtensions
	{
		public static string GetUserName(this HubCallerContext context)
		{
			if (!(context.User.Identity is ClaimsIdentity principal))
				throw new UnauthorizedAccessException("Invalid ClaimsIdentity");

			var nameIdentifier = principal.Claims.GetClaimValueAsString(ClaimTypes.Name);
			if (nameIdentifier == null)
				throw new UnauthorizedAccessException("NameIdentifier not found");

			return nameIdentifier;
		}

		public static int GetUserId(this HubCallerContext context)
		{
			if (!(context.User.Identity is ClaimsIdentity principal))
				throw new UnauthorizedAccessException("Invalid ClaimsIdentity");

			var nameIdentifier = principal.Claims.GetClaimValueAsInt(ClaimTypes.NameIdentifier);
			if (nameIdentifier == null)
				throw new UnauthorizedAccessException("NameIdentifier not found");

			return nameIdentifier.Value;
		}

		public static bool IsAuthenticated(this HubCallerContext context)
		{
			return context.User.Identity.IsAuthenticated;
		}

		public static string GetIPAddress(this HubCallerContext context)
		{
			return context.Request.GetHttpContext().Request.GetIPAddress();
		}

		public static dynamic User(this IHubConnectionContext<dynamic> client, int userId)
		{
			return client.User(userId.ToString());
		}

		public static dynamic Group(this IHubConnectionContext<dynamic> client, int groupId)
		{
			return client.Group(groupId.ToString());
		}



		public static Claim GetClaim(this IEnumerable<Claim> claims, string name)
		{
			if (claims != null && claims.Any())
				return claims.First(x => x.Type == name);

			return null;
		}

		public static string GetClaimValueAsString(this IEnumerable<Claim> claims, string name)
		{
			if (claims != null && claims.Any())
				return claims.First(x => x.Type == name).Value;

			return string.Empty;
		}

		public static DateTime? GetClaimValueAsDateTime(this IEnumerable<Claim> claims, string name)
		{
			if (claims != null && claims.Any())
				return DateTime.TryParse(claims.First(x => x.Type == name).Value, out DateTime returnValue)
				? returnValue
				: default(DateTime?);

			return default(DateTime?);
		}

		public static int? GetClaimValueAsInt(this IEnumerable<Claim> claims, string name)
		{
			if (claims != null && claims.Any())
				return int.TryParse(claims.First(x => x.Type == name).Value, out int returnValue)
				? returnValue
				: default(int?);

			return default(int?);
		}

		public static bool? GetClaimValueAsBool(this IEnumerable<Claim> claims, string name)
		{
			if (claims != null && claims.Any())
				return bool.TryParse(claims.First(x => x.Type == name).Value, out bool returnValue)
				? returnValue
				: default(bool?);

			return default(bool?);
		}

		public static Guid? GetClaimValueAsGuid(this IEnumerable<Claim> claims, string name)
		{
			if (claims != null && claims.Any())
				return Guid.TryParse(claims.First(x => x.Type == name).Value, out Guid returnValue)
				? returnValue
				: default(Guid?);

			return default(Guid?);
		}

		public static T? GetClaimValueAsEnum<T>(this IEnumerable<Claim> claims, string name) where T : struct
		{
			if (claims != null && claims.Any())
				return Enum.TryParse(claims.First(x => x.Type == name).Value, out T returnValue)
				? returnValue
				: default(T?);

			return default(T?);
		}

		public static bool IsAuthenticated(this ClaimsPrincipal principal)
		{
			return principal?.Identity != null && principal.Identity.IsAuthenticated;
		}
	}
}
