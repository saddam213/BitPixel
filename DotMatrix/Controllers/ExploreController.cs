using System.Threading.Tasks;
using System.Web.Mvc;
using DotMatrix.Common.Explore;
using DotMatrix.Common.Game;
using DotMatrix.Enums;

namespace DotMatrix.Controllers
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