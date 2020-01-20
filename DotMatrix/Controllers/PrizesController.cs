using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

using DotMatrix.Common.Payment;
using DotMatrix.Common.Points;
using DotMatrix.Common.Prize;
using DotMatrix.Common.Users;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class PrizesController : Controller
	{
		public IPrizeReader PrizeReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new PrizeViewModel
			{
				Prizes = await PrizeReader.GetPrizes()
			});
		}

		[HttpGet]
		public async Task<ActionResult> History()
		{
			var prizes = await PrizeReader.GetPrizeHistory();
			return View(new PrizeHistoryViewModel
			{
				Prizes = prizes
					.OrderByDescending(x => x.Timestamp)
					.Take(1000)
					.ToList()
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UserHistory()
		{
			var userPrizes = await PrizeReader.GetUserPrizes(User.Identity.GetUserId<int>());
			return View(new PrizeUserHistoryViewModel
			{
				Prizes = userPrizes
					.OrderByDescending(x => x.Timestamp)
					.ToList()
			});
		}
	}
}