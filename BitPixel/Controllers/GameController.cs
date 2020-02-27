using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BitPixel.Cache.Common;
using BitPixel.Common.Game;
using BitPixel.Common.Pixel;
using BitPixel.Common.Team;
using BitPixel.Common.Users;
using BitPixel.Enums;
using BitPixel.Helpers;
using Microsoft.AspNet.Identity;

namespace BitPixel.Controllers
{
	public class GameController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IGameWriter GameWriter { get; set; }
		public IUserReader UserReader { get; set; }
		public IPixelReader PixelReader { get; set; }

		public async Task<ActionResult> Index(int gameId)
		{
			var userTeam = default(TeamModel);
			var teams = new List<TeamModel>();
			var userId = User.Identity.GetUserId<int>();
			var game = await GameReader.GetGame(gameId);
			if (game.Status == GameStatus.Finished)
				return RedirectToAction("Game", "Gallery", new { gameId = gameId });

			var user = await UserReader.GetUser(userId);
			if (game.Type == GameType.TeamBattle)
			{
				teams = await GameReader.GetTeams(game.Id);
				userTeam = await GameReader.GetUserTeam(userId, game.Id);
			}

			return View(new PixelViewlModel
			{
				Game = game,
				Points = user?.Points ?? 0,
				Teams = teams,
				Team = userTeam
			});
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels(int gameId)
		{
			return Json(await PixelReader.GetPixels(gameId), JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> ChangeTeamModal(int gameId)
		{
			return View(await PopulateChangeTeamModel(new ChangeTeamModel
			{
				GameId = gameId
			}));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeTeamModal(ChangeTeamModel model)
		{
			var userId = User.Identity.GetUserId<int>();
			if (!ModelState.IsValid)
				return View(await PopulateChangeTeamModel(model));


			var result = await GameWriter.ChangeTeam(userId, model);
			if (!ModelState.IsWriterResultValid(result))
				return View(await PopulateChangeTeamModel(model));

			return CloseModalSuccess();
		}

		private async Task<ChangeTeamModel> PopulateChangeTeamModel(ChangeTeamModel model)
		{
			var userId = User.Identity.GetUserId<int>();
			var userTeam = await GameReader.GetUserTeam(userId, model.GameId);
			model.Teams = await GameReader.GetTeams(model.GameId);
			model.TeamId = userTeam?.Id;
			return model;
		}
	}
}