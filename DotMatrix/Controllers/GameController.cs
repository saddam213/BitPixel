using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotMatrix.Cache.Common;
using DotMatrix.Common.Game;
using DotMatrix.Common.Pixel;
using DotMatrix.Common.Users;
using DotMatrix.Enums;
using DotMatrix.Helpers;
using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class GameController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IUserReader UserReader { get; set; }
		public IPixelWriter PixelWriter { get; set; }
		public IPixelReader PixelReader { get; set; }
		public IThrottleCache ThrottleCache { get; set; }

		public async Task<ActionResult> Index(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			var user = await UserReader.GetUser(User.Identity.GetUserId<int>());
			return View(new PixelViewlModel
			{
				Game = game,
				Points = user?.Points ?? 0
			});
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddPixel(AddPixelRequest model)
		{
			if (!ModelState.IsValid)
				return Json(new { Success = false });

			var game = await GameReader.GetGame(model.GameId);
			if (game == null)
				return Json(new { Success = false, Message = "Game not found" });

			if (game.Status != GameStatus.Started)
				return Json(new { Success = false, Message = "Game is not currently active" });

			var userId = User.Identity.GetUserId<int>();
			var rateLimitResult = await CheckRateLimits(userId, game);
			if (!string.IsNullOrEmpty(rateLimitResult))
				return Json(new { Success = false, Message = rateLimitResult });

			var result = await PixelWriter.AddPixel(userId, model);
			if (!result.Success)
				return Json(new { Success = false, Message = result.Message });

			await PixelHub.UpdateFrontEndPixel(new PixelHubAddPixelRequest
			{
				X = model.X,
				Y = model.Y,
				Color = model.Color,
				Type = PixelType.User,

				PixelId = result.PixelId,
				UserId = result.UserId,
				UserName = result.UserName,
				UserPoints = result.UserPoints,
				TeamId = result.TeamId,
				TeamName = result.TeamName,
				NewPoints = result.NewPoints,

				GameId = result.GameId,
				GameName = result.GameName,
				IsApi = false
			});

			if (result.IsPrizeWinner)
			{
				await PixelHub.UpdateFrontEndPrize(new PixelHubAddPrizeRequest
				{
					X = model.X,
					Y = model.Y,
					UserId = userId,
					PrizeId = result.PrizeId,
					PrizeName = result.PrizeName,
					PrizeUser = User.Identity.Name,
					PrizePoints = result.PrizePoints,
					PrizeDescription = result.PrizeDescription,
					UserPoints = result.UserPoints,
					GameId = result.GameId,
					GameName = result.GameName
				});
			}

			return Json(new { Success = true });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPixel(GetPixelRequest model)
		{
			if (!ModelState.IsValid)
				return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

			var game = await GameReader.GetGame(model.GameId);
			if (game == null)
				return Json(new { Success = false, Message = "Game not found" });

			if (User.Identity.IsAuthenticated)
			{
				if (game.Status == GameStatus.Started)
				{
					var userId = User.Identity.GetUserId<int>();
					var rateLimitResult = await CheckRateLimits(userId, game);
					if (!string.IsNullOrEmpty(rateLimitResult))
						return Json(new { Success = false, Message = rateLimitResult });

					var clickResult = await PixelWriter.AddClick(userId, new AddClickRequest(model.GameId, model.X, model.Y));
					if (!clickResult.Success)
						return Json(new { Success = false, Message = clickResult.Message });
				}
			}

			var result = await PixelReader.GetPixel(model.GameId, model.X, model.Y);
			if (result == null)
				return Json(new { Success = false, Message = "Pixel not found." }, JsonRequestBehavior.AllowGet);

			return Json(new
			{
				Success = true,
				Data = result
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels(int gameId)
		{
			return Json(await PixelReader.GetPixels(gameId), JsonRequestBehavior.AllowGet);
		}


		private async Task<string> CheckRateLimits(int userId, GameModel game)
		{
			if (!await ThrottleCache.ShouldExcecute(userId, $"GameClicks:{game.Id}", TimeSpan.FromSeconds(1), game.ClicksPerSecond))
				return $"Maximum {game.ClicksPerSecond} clicks per second";

			if (!await ThrottleCache.ShouldExcecute(userId, "UserClicks", TimeSpan.FromHours(24), Constant.ClicksPerDay))
				return $"Maximum {Constant.ClicksPerDay} clicks per day";

			if (!await ThrottleCache.ShouldExcecute(Request.GetIPAddress(), "IPAddressClicks", TimeSpan.FromHours(24), Constant.ClicksPerDay))
				return $"Maximum {Constant.ClicksPerDay} clicks per day (ip)";

			return string.Empty;
		}
	}
}