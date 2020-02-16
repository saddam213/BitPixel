using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Game;
using DotMatrix.Common.Home;

namespace DotMatrix.Controllers
{
	public class HomeController : BaseController
	{
		public IGameReader GameReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var games = await GameReader.GetGames();
			return View(new HomeViewModel
			{
				Games = games
				 .Where(x => x.Status != Enums.GameStatus.Deleted)
				 .ToList(),
			});
		}

		//public Task<ActionResult> Api()
		//{
		//	return Task.FromResult<ActionResult>(View());
		//}
	}
}