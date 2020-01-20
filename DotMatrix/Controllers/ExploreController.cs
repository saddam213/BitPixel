using System.Threading.Tasks;
using System.Web.Mvc;

namespace DotMatrix.Controllers
{
	public class ExploreController : Controller
	{
		public Task<ActionResult> Index()
		{
			return Task.FromResult<ActionResult>(View());
		}
	}
}