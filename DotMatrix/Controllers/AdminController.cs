using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using DotMatrix.Common.Admin;
using DotMatrix.Common.Award;
using DotMatrix.Common.Game;
using DotMatrix.Common.Image;
using DotMatrix.Common.Payment;
using DotMatrix.Common.Prize;
using DotMatrix.Common.Users;
using DotMatrix.Enums;
using DotMatrix.Helpers;

namespace DotMatrix.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public IUserWriter UserWriter { get; set; }
		public IGameReader GameReader { get; set; }
		public IGameWriter GameWriter { get; set; }
		public IPrizeReader PrizeReader { get; set; }
		public IPrizeWriter PrizeWriter { get; set; }
		public IPaymentReader PaymentReader { get; set; }
		public IPaymentWriter PaymentWriter { get; set; }
		public IImageWriter ImageWriter { get; set; }
		public IAwardReader AwardReader { get; set; }
		public IAwardWriter AwardWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new AdminViewModel());
		}


		[HttpGet]
		public async Task<ActionResult> Game()
		{
			return View(new AdminGameViewModel
			{
				Games = await GameReader.GetGames(),
				Prizes = await PrizeReader.GetPrizes(),

			});
		}


		[HttpGet]
		public async Task<ActionResult> Users()
		{
			return View(new AdminUserViewModel
			{
				Users = await UserReader.GetUsers()
			});
		}

		[HttpGet]
		public async Task<ActionResult> Award()
		{
			return View(new AdminAwardViewModel
			{
				Awards = await AwardReader.GetAwards()
			});
		}

		[HttpGet]
		public async Task<ActionResult> Payment()
		{
			return View(new AdminPaymentViewModel
			{
				Payments = await PaymentReader.GetReceipts(),
				PrizePayments = await PrizeReader.GetPrizePayments()
			});
		}

		[HttpGet]
		public async Task<ActionResult> CreatePrizePoolModal()
		{
			return View(new CreatePrizePoolModel
			{
				Games = await GameReader.GetGames(Enums.GameStatus.NotStarted)
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreatePrizePoolModal(CreatePrizePoolModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Games = await GameReader.GetGames(Enums.GameStatus.NotStarted);
				return View(model);
			}


			if (model.Type == Enums.PrizeType.Points)
			{
				model.Data = null;
				model.Data2 = null;
			}
			else if (model.Type == Enums.PrizeType.Crypto)
			{
				var isValid = true;
				if (string.IsNullOrEmpty(model.Data))
				{
					// coin symbol missing
					ModelState.AddModelError("", "Coin symbol missing");
					isValid = false;
				}

				if (!decimal.TryParse(model.Data2, out var parsedAmount) || parsedAmount < 0.00000001m)
				{
					// invalid amount
					ModelState.AddModelError("", "Invalid amount");
					isValid = false;
				}

				if (!isValid)
				{
					model.Games = await GameReader.GetGames(Enums.GameStatus.NotStarted);
					return View(model);
				}
			}

			var result = await PrizeWriter.CreatePrizePool(model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Games = await GameReader.GetGames(Enums.GameStatus.NotStarted);
				return View(model);
			}
			return CloseModalSuccess();
		}



		[HttpGet]
		public async Task<ActionResult> CreateGameModal()
		{
			return View(new CreateGameModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateGameModal(CreateGameModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await GameWriter.CreateGame(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}



		[HttpGet]
		public async Task<ActionResult> UpdateGameModal(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new UpdateGameModel
			{
				Id = game.Id,
				Name = game.Name,
				Description = game.Description,
				Status = game.Status,
				Rank = game.Rank,
				ClicksPerSecond = game.ClicksPerSecond,
				Platform = game.Platform
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateGameModal(UpdateGameModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await GameWriter.UpdateGame(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}


		[HttpGet]
		public async Task<ActionResult> UpdatePrizePoolModal(int gameId, string name)
		{
			var prizes = await PrizeReader.GetPrizes(gameId);
			var prize = prizes.FirstOrDefault(x => x.Name == name);
			return View(new UpdatePrizePoolModel
			{
				GameId = gameId,
				Name = prize.Name,
				Description = prize.Description,
				NewName = prize.Name
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePrizePoolModal(UpdatePrizePoolModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await PrizeWriter.UpdatePrizePool(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}






		[HttpGet]
		public async Task<ActionResult> UpdatePaymentModal(int paymentId)
		{
			var payment = await PaymentReader.GetReceipt(paymentId);
			return View(new UpdatePaymentModel
			{
				Id = payment.Id,
				Name = payment.Name,
				Points = payment.Points,
				Rate = payment.Rate,
				Status = payment.Status,
				Updated = payment.Updated,
				UserName = payment.UserName,
				Amount = payment.Amount,
				Created = payment.Created,
				Data2 = payment.Data2,
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePaymentModal(UpdatePaymentModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await PaymentWriter.UpdatePayment(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}




		[HttpGet]
		public async Task<ActionResult> UpdateUserModal(int userId)
		{
			var user = await UserReader.GetUser(userId);
			return View(new UpdateUserModal
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				IsApiEnabled = user.IsApiEnabled,
				Points = user.Points,
				IsEmailConfirmed = user.IsEmailConfirmed,
				IsLocked = user.IsLocked
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateUserModal(UpdateUserModal model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await UserWriter.UpdateUser(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}






		[HttpGet]
		public async Task<ActionResult> UpdatePrizePaymentModal(int prizeId)
		{
			var prize = await PrizeReader.GetPrizePayment(prizeId);
			return View(new UpdatePrizePaymenModel
			{
				Id = prize.Id,
				Name = prize.Name,
				Status = prize.Status,
				Data = prize.Data,
				Data2 = prize.Data2,
				Data3 = prize.Data3,
				Data4 = prize.Data4,
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePrizePaymentModal(UpdatePrizePaymenModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await PrizeWriter.UpdatePrizePayment(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}








		[HttpGet]
		public async Task<ActionResult> CreateFixedImageModal(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			return View(new CreateFixedImageModel
			{
				GameId = game.Id
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateFixedImageModal(CreateFixedImageModel model, HttpPostedFileBase imageFile)
		{
			if (!ModelState.IsValid)
				return View(model);

			if (!imageFile.IsValidImage())
			{
				ModelState.AddModelError("", "Invalid image upload");
				return View(model);
			}

			model.ImageStream = imageFile.InputStream;
			var result = await ImageWriter.CreateFixedImage(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}



		[HttpGet]
		public async Task<ActionResult> UpdateAwardModal(int awardId)
		{
			var award = await AwardReader.GetAward(awardId);
			return View(new UpdateAwardModel
			{
				Id = award.Id,
				Name = award.Name,
				Description = award.Description,
				Icon = award.Icon,
				Level = award.Level,
				Points = award.Points,
				Rank = award.Rank,
				Status = award.Status
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateAwardModal(UpdateAwardModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await AwardWriter.UpdateAward(model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalSuccess();
		}

	}
}