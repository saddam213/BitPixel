using System.Threading.Tasks;
using System.Web.Mvc;
using BitPixel.Common.Explore;
using BitPixel.Common.Game;
using BitPixel.Enums;

namespace BitPixel.Controllers
{
	public class ExploreController : BaseController
	{
		public IGameReader GameReader { get; set; }

		public async Task<ActionResult> Index(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new ExploreViewModel
			{
				Game = game
			});
		}
	}
}