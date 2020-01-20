using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Score;

namespace DotMatrix.Controllers
{
	public class ScoresController : Controller
	{
		public IScoreReader ScoreReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(await ScoreReader.GetScoreboard());
		}
	}
}