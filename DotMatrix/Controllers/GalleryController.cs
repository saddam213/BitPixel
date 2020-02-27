using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotMatrix.Common.Gallery;
using DotMatrix.Common.Game;
using DotMatrix.Common.Team;
using DotMatrix.Enums;

namespace DotMatrix.Controllers
{
	public class GalleryController : BaseController
	{
		public IGameReader GameReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var games = await GameReader.GetGames();
			return View(new GalleryViewModel
			{
				Games = games
				 .Where(x => x.Status == Enums.GameStatus.Finished)
				 .ToList(),
			});
		}

		public async Task<ActionResult> Game(int gameId)
		{
			var teams = new List<TeamModel>();
			var game = await GameReader.GetGame(gameId);
			var players = await GameReader.GetPlayers(gameId);
			if (game.Type == GameType.TeamBattle)
				teams = await GameReader.GetTeams(game.Id);

			return View(new GalleryGameViewModel
			{
				Game = game,
				Players = players,
				Teams = teams,
				Team = teams
					.OrderBy(x => x.Result)
					.FirstOrDefault()
			});

		}
	}
}