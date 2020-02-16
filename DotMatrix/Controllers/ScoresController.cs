using System.Threading.Tasks;
using System.Web.Mvc;
using DotMatrix.Common.Game;
using DotMatrix.Common.Score;
using DotMatrix.Enums;

namespace DotMatrix.Controllers
{
	public class ScoresController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IScoreReader ScoreReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new ScoresViewModel
			{
				Scores = await ScoreReader.GetScoreboard()
			});
		}

		[HttpGet]
		public async Task<ActionResult> Game(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new GameScoresViewModel
			{
				Game = game,
				Scores = await ScoreReader.GetScoreboard(game.Id)
			});
		}
	}
}