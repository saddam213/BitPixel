using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using DotMatrix.Api;
using DotMatrix.Common.Account;
using DotMatrix.Common.Api;
using DotMatrix.Common.Award;
using DotMatrix.Common.Image;
using DotMatrix.Common.Users;
using DotMatrix.Helpers;
using DotMatrix.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DotMatrix.Controllers
{
	public class UserController : BaseController
	{
		private ApplicationUserManager _userManager;

		public UserController()
		{
		}

		public UserController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
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
		public IImageWriter ImageWriter { get; set; }
		public IAwardWriter AwardWriter { get; set; }

		public async Task<ActionResult> Index()
		{
			var user = await UserReader.GetUser(User.Identity.GetUserId<int>());
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

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
				if (user != null)
				{
					//await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
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

			var result = await ApiKeyStore.UpdateApiAuthKey(User.Identity.GetUserId<int>(), model);
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
				return Json(new { Success = false, Message = "Failed to generate API key, if problem persists please contact Support." });

			return Json(new
			{
				Success = true,
				Key = result.Key,
				Secret = result.Secret
			});
		}

		[HttpGet]
		[Authorize]
		public ActionResult UpdateAvatarModal()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> UpdateAvatarModal(UpdateAvatarModel model)
		{
			var userId = User.Identity.GetUserId<int>();
			model.UserName = User.Identity.Name;
			model.AvatarPath = Server.MapPath("~/Content/Images/Avatar");
			var result = await ImageWriter.CreateAvatarImage(model);
			if (result.Success)
			{
				await AwardWriter.AddUserAward(new AddUserAwardModel
				{
					UserId = userId,
					Type = Enums.AwardType.Avatar
				});
			}
			return Json(result);
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