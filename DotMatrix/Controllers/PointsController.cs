using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

using DotMatrix.Common.Payment;
using DotMatrix.Common.Points;
using DotMatrix.Common.Prize;
using DotMatrix.Common.Users;

using Microsoft.AspNet.Identity;
using DotMatrix.Common.Award;

namespace DotMatrix.Controllers
{
	public class PointsController : Controller
	{
		public IUserReader UserReader { get; set; }
		public IPrizeReader PrizeReader { get; set; }
		public IAwardReader AwardReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Index()
		{
			var userId = User.Identity.GetUserId<int>();
			var user = await UserReader.GetUser(userId);
			var userPrizes = await PrizeReader.GetUserPrizes(userId);
			var userAwards = await AwardReader.GetUserAwards(userId);
			return View(new PointsModel
			{
				Points = user.Points,
				LatestPrizes = userPrizes
					.OrderByDescending(x => x.Timestamp)
					.Take(10)
					.ToList(),
				LatestAwards = userAwards
					.OrderByDescending(x => x.Timestamp)
					.Take(10)
					.ToList()
			});
		}

	}
}