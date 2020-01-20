using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Admin;

namespace DotMatrix.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		public IAdminWriter AdminWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreatePrizePool(CreatePrizePoolModel model)
		{
			if (model.Type == Enums.PrizeType.Points)
			{
				model.Data = null;
				model.Data2 = null;
			}
			else if (model.Type == Enums.PrizeType.Crypto)
			{
				if (string.IsNullOrEmpty(model.Data))
				{
					// coin symbol missing
				}

				if (!decimal.TryParse(model.Data2, out var parsedAmount) || parsedAmount < 0.00000001m)
				{
					// invalid amount
				}
			}

			await AdminWriter.CreatePrizePool(model);
			return RedirectToAction("Index");
		}


	}
}