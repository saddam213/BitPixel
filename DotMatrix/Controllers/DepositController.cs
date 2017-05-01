using DotMatrix.Common.Deposits;
using DotMatrix.Common.Users;
using DotMatrix.Common.Wallet;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DotMatrix.Controllers
{
	public class DepositController : Controller
	{
		public IUserReader UserReader { get; set; }
		public IDepositReader DepositReader { get; set; }
		public IWalletService WalletService { get; set; }

		[Authorize]
		public async Task<ActionResult> Index()
		{
			var user = await UserReader.GetUser(User.Identity.GetUserId());
			var deposits = await DepositReader.GetDeposits(User.Identity.GetUserId());
			return View(new DepositsModel
			{
				Address = user.Address,
				Balance = user.Balance,
				Deposits = deposits
			});
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> GenerateAddress()
		{
			var result = await WalletService.GenerateAddress(User.Identity.GetUserId());
			if (string.IsNullOrEmpty(result))
				return Json(new { Success = false, Meaage = "Failed to generate Dotcoin address, if problem persists please contact Support." });

			return Json(new
			{
				Success = true,
				Address = result
			});
		}
	}
}