using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using DotMatrix.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using DotMatrix.Api;
using DotMatrix.Common.Api;
using DotMatrix.Common.Users;
using DotMatrix.Common.Deposits;
using DotMatrix.Common.Wallet;

namespace DotMatrix.Controllers
{
	public class UserController : Controller
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public UserController()
		{
		}

		public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		public IUserReader UserReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var user = await UserReader.GetUser(User.Identity.GetUserId());
			return View(new UserSettingsModel
			{
				ApiModel = new UpdateApiModel
				{
					ApiKey = user.ApiKey,
					ApiSecret = user.ApiSecret,
					IsApiEnabled = user.IsApiEnabled
				}
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_UpdatePassword", model);
			}

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}

				ModelState.AddModelError("Success", "Successfully updated password.");
				return PartialView("_UpdatePassword", model);
			}
			AddErrors(result);
			return PartialView("_UpdatePassword", model);
		}


		[HttpPost]
		[Authorize]
		public async Task<ActionResult> UpdateApiSettings(UpdateApiModel model)
		{
			if (!ModelState.IsValid)
				return PartialView("_UpdateApi", model);

			var result = await ApiKeyStore.UpdateApiAuthKey(User.Identity.GetUserId(), model);
			if (!result)
			{
				ModelState.AddModelError("Error", "Failed to update API key.");
				return PartialView("_UpdateApi", model);
			}

			ModelState.AddModelError("Success", "Successfully updated API key.");
			return PartialView("_UpdateApi", model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult GenerateApiKey()
		{
			var result = ApiKeyStore.GenerateApiKeyPair();
			if (!result.IsValid)
				return Json(new { Success = false, Meaage = "Failed to generate API key, if problem persists please contact Support." });

			return Json(new
			{
				Success = true,
				Key = result.Key,
				Secret = result.Secret
			});
		}



		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}
	}
}