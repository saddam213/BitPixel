using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using BitPixel.Common.Account;
using BitPixel.Common.Award;
using BitPixel.Common.Image;
using BitPixel.Common.Users;
using BitPixel.Identity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BitPixel.Controllers
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
			return View(new UserSettingsModel());
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