using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Game;
using DotMatrix.Common.Replay;

namespace DotMatrix.Controllers
{
	public class ReplayController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IReplayReader ReplayReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			var players = await GameReader.GetPlayers(gameId);
			return View(new ReplayViewModel
			{
				GameId = game.Id,
				Width = game.Width,
				Height = game.Height,
				Players = players
			});
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels(ReplayFilterModel model)
		{
			return Json(await ReplayReader.GetPixels(model), JsonRequestBehavior.AllowGet);
		}
	}
}