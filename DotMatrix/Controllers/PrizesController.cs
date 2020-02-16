using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Game;
using DotMatrix.Common.Prize;
using DotMatrix.Datatables;
using DotMatrix.Enums;
using DotMatrix.Helpers;
using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class PrizesController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IPrizeReader PrizeReader { get; set; }
		public IPrizeWriter PrizeWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new PrizesViewModel
			{
				Prizes = await PrizeReader.GetPrizes()
			});
		}

		[HttpGet]
		public async Task<ActionResult> ViewPrizesModal(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new ViewPrizesModalModel
			{
				Game = game,
				Prizes = await PrizeReader.GetPrizes(game.Id)
			});
		}

		[HttpGet]
		public async Task<ActionResult> ViewPrizeModal(int prizeId)
		{
			var prize = await PrizeReader.GetPrize(prizeId);
			var game = await GameReader.GetGame(prize.GameId);
			return View(new ViewPrizeModalModel
			{
				Game = game,
				Prize = prize
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> ViewUserPrizeModal(int prizeId)
		{
			var userId = User.Identity.GetUserId<int>();
			var prize = await PrizeReader.GetUserPrize(userId, prizeId);
			var game = await GameReader.GetGame(prize.GameId);
			return View(new ViewUserPrizeModalModel
			{
				Game = game,
				Prize = prize
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> ClaimPrizeModal(int prizeId)
		{
			var userId = User.Identity.GetUserId<int>();
			var prize = await PrizeReader.GetUserPrize(userId, prizeId);
			return View(new ClaimPrizeModel
			{
				Id = prize.Id,
				Name = prize.Name,
				Description = prize.Description,
				Game = prize.Game,
				Points = prize.Points,
				Status = prize.Status,
				Type = prize.Type,
				X = prize.X,
				Y = prize.Y,

				Data = prize.Data,
				Data2 = prize.Data2,
				Data3 = prize.Data3,
				Data4 = prize.Data4
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ClaimPrizeModal(ClaimPrizeModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var userId = User.Identity.GetUserId<int>();
			var prize = await PrizeReader.GetUserPrize(userId, model.Id);
			if (prize.Status != PrizeStatus.Unclaimed)
				return CloseModalSuccess();

			var result = await PrizeWriter.ClaimPrize(userId, model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}

		#region All History

		[HttpGet]
		public Task<ActionResult> History()
		{
			return Task.FromResult<ActionResult>(View());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetHistory(DataTablesParam model)
		{
			return DataTable(await PrizeReader.GetHistory(model));
		}

		#endregion

		#region Game History

		[HttpGet]
		public async Task<ActionResult> GameHistory(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new PrizeGameHistoryViewModel
			{
				Id = game.Id,
				Name = game.Name
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetGameHistory(DataTablesParam model, int gameId)
		{
			return DataTable(await PrizeReader.GetGameHistory(model, gameId));
		}

		#endregion

		#region User History

		[HttpGet]
		[Authorize]
		public Task<ActionResult> UserHistory()
		{
			return Task.FromResult<ActionResult>(View());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetUserHistory(DataTablesParam model)
		{
			return DataTable(await PrizeReader.GetUserHistory(model, User.Identity.GetUserId<int>(), null));
		}

		#endregion
	}
}