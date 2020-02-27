using System.Threading.Tasks;
using System.Web.Mvc;
using BitPixel.Common.Game;
using BitPixel.Common.Score;
using BitPixel.Enums;

namespace BitPixel.Controllers
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