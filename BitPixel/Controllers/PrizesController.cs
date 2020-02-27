using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using BitPixel.Common.Game;
using BitPixel.Common.Payment;
using BitPixel.Common.Prize;
using BitPixel.Datatables;
using BitPixel.Enums;
using BitPixel.Helpers;
using Microsoft.AspNet.Identity;

namespace BitPixel.Controllers
{
	public class PrizesController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IPrizeReader PrizeReader { get; set; }
		public IPrizeWriter PrizeWriter { get; set; }
		public IPaymentReader PaymentReader { get; set; }
		public IPaymentWriter PaymentWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var prizes = await PrizeReader.GetPrizes();
			return View(new PrizesViewModel
			{
				Prizes = prizes
					.Where(x => x.GameStatus != GameStatus.Finished)
					.ToList()
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
			if (prize.Type != PrizeType.Crypto)
				return RedirectToAction("ViewUserPrizeModal", new { prizeId = prizeId });

			var paymentMethod = await PaymentReader.GetMethod(prize.Data);
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
				Data4 = prize.Data4,

				Rate = paymentMethod.Rate,
				Amount = decimal.Parse(prize.Data2)
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

			if (model.IsPointsClaim)
			{
				var paymentMethod = await PaymentReader.GetMethod(prize.Data);
				if (paymentMethod == null)
					return CloseModalError("Unknown Error");

				var paymentUserMethod = await PaymentReader.GetUserMethod(userId, paymentMethod.Id);
				if (paymentUserMethod == null)
					await PaymentWriter.CreateMethod(userId, paymentMethod.Id);

				paymentUserMethod = await PaymentReader.GetUserMethod(userId, paymentMethod.Id);
				if (paymentUserMethod == null)
					return CloseModalError("Unknown Error");

				model.Data3 = paymentUserMethod.Data;
			}

			if (string.IsNullOrEmpty(model.Data3))
			{
				ModelState.AddModelError("", "Invalid Crypto Address");
				return View(model);
			}

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