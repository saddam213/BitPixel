using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

using DotMatrix.Common.Payment;
using DotMatrix.Common.Points;
using DotMatrix.Common.Award;
using DotMatrix.Common.Users;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class AwardsController : Controller
	{
		public IAwardReader AwardReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new AwardViewModel
			{
				Awards = await AwardReader.GetAwards()
			});
		}

		[HttpGet]
		public async Task<ActionResult> History()
		{
			var awards = await AwardReader.GetAwardHistory();
			return View(new AwardHistoryViewModel
			{
				Awards = awards
					.OrderByDescending(x => x.Timestamp)
					.Take(1000)
					.ToList()
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UserHistory()
		{
			var userAwards = await AwardReader.GetUserAwards(User.Identity.GetUserId<int>());
			return View(new AwardUserHistoryViewModel
			{
				Awards = userAwards
					.OrderByDescending(x => x.Timestamp)
					.ToList()
			});
		}
	}
}