using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

using BitPixel.Common.Payment;
using BitPixel.Common.Points;
using BitPixel.Common.Prize;
using BitPixel.Common.Users;

using Microsoft.AspNet.Identity;
using BitPixel.Common.Award;
using BitPixel.Datatables;
using BitPixel.Common.Pixel;
using BitPixel.Common.Click;

namespace BitPixel.Controllers
{
	public class PointsController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public IPrizeReader PrizeReader { get; set; }
		public IAwardReader AwardReader { get; set; }
		public IPixelReader PixelReader { get; set; }
		public IClickReader ClickReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Index()
		{
			var userId = User.Identity.GetUserId<int>();
			var user = await UserReader.GetUser(userId);
			var userAwards = await AwardReader.GetUserAwardList(userId);
			return View(new PointsModel
			{
				Points = user.Points,
				LatestPrizes = new System.Collections.Generic.List<PrizeUserHistoryItemModel>(),
				AwardList = userAwards
			});
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPrizeSummary(DataTablesParam model)
		{
			return DataTable(await PrizeReader.GetUserHistory(model, User.Identity.GetUserId<int>(), 10));
		}


		[HttpGet]
		[Authorize]
		public Task<ActionResult> ClickHistory()
		{
			return Task.FromResult<ActionResult>(View());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetClickHistory(DataTablesParam model)
		{
			return DataTable(await ClickReader.GetUserHistory(model, User.Identity.GetUserId<int>(), null));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetClickSummary(DataTablesParam model)
		{
			return DataTable(await ClickReader.GetUserHistory(model, User.Identity.GetUserId<int>(), 10));
		}



		[HttpGet]
		[Authorize]
		public Task<ActionResult> PixelHistory()
		{
			return Task.FromResult<ActionResult>(View());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPixelHistory(DataTablesParam model)
		{
			return DataTable(await PixelReader.GetUserHistory(model, User.Identity.GetUserId<int>(), null));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPixelSummary(DataTablesParam model)
		{
			return DataTable(await PixelReader.GetUserHistory(model, User.Identity.GetUserId<int>(), 10));
		}
	}
}