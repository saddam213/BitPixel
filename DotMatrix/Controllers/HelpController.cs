using System.Threading.Tasks;
using System.Web.Mvc;

namespace DotMatrix.Controllers
{
	public class HelpController : BaseController
	{
		[HttpGet]
		public Task<ActionResult> Index()
		{
			return Task.FromResult<ActionResult>(View());
		}
	}
}