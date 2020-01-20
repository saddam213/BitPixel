using System.Threading.Tasks;
using System.Web.Mvc;

namespace DotMatrix.Controllers
{
	public class HomeController : Controller
	{

		public Task<ActionResult> Index()
		{
			return Task.FromResult<ActionResult>(View());
		}

		//public Task<ActionResult> Api()
		//{
		//	return Task.FromResult<ActionResult>(View());
		//}
	}
}