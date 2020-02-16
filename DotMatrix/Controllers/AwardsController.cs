using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Award;
using DotMatrix.Common.Game;
using DotMatrix.Datatables;
using DotMatrix.Enums;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class AwardsController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IAwardReader AwardReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new AwardViewModel
			{
				AwardList = await AwardReader.GetAwardList()
			});
		}


		[HttpGet]
		public Task<ActionResult> Search()
		{
			return Task.FromResult<ActionResult>(View());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetSearch(DataTablesParam model)
		{
			return DataTable(await AwardReader.GetAwards(model));
		}


		[HttpGet]
		public Task<ActionResult> History()
		{
			return Task.FromResult<ActionResult>(View());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetHistory(DataTablesParam model)
		{
			return DataTable(await AwardReader.GetHistory(model));
		}


		[HttpGet]
		[Authorize]
		public Task<ActionResult> UserHistory(string award)
		{
			return Task.FromResult<ActionResult>(View(new AwardUserHistoryViewModel
			{
				SearchName = award
			}));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetUserHistory(DataTablesParam model)
		{
			return DataTable(await AwardReader.GetUserHistory(model, User.Identity.GetUserId<int>()));
		}



		[HttpGet]
		public async Task<ActionResult> GameHistory(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new AwardGameHistoryViewModel
			{
				Id = game.Id,
				Name = game.Name
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetGameHistory(DataTablesParam model, int gameId)
		{
			return DataTable(await AwardReader.GetGameHistory(model, gameId));
		}


		[HttpGet]
		public async Task<ActionResult> ViewAwardModal(int awardId)
		{
			var award = await AwardReader.GetAward(awardId);
			return View(new AwardModalViewModel
			{
				Id = award.Id,
				Icon = award.Icon,
				Name = award.Name,
				Points = award.Points,
				ClickType = award.ClickType,
				Description = award.Description,
				Level = award.Level,
				TriggerType = award.TriggerType
			});
		}


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> ViewUserAwardModal(int awardId)
		{
			var userId = User.Identity.GetUserId<int>();
			var award = await AwardReader.GetUserAward(userId, awardId);
			return View(new AwardUserModalViewModel
			{
				Id = award.Id,
				Icon = award.Icon,
				Name = award.Name,
				Points = award.Points,
				Count = award.Count
			});
		}


	}
}