using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using DotMatrix.Common.Account;
using DotMatrix.Common.Award;
using DotMatrix.Common.Email;
using DotMatrix.Enums;
using DotMatrix.Helpers;
using DotMatrix.Identity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace DotMatrix.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		private ApplicationUserManager _userManager;

		public AccountController()		{		}
		public AccountController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
			private set { _userManager = value; }
		}

		public IEmailService EmailService { get; set; }
		public IAwardWriter AwardWriter { get; set; }
		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!CaptchaHelper.Validate())
			{
				ModelState.AddModelError("", "Invalid Captcha");
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Check User
			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				ModelState.AddModelError("", "Incorrect Email or Password");
				return View(model);
			}

			if (!await UserManager.IsEmailConfirmedAsync(user.Id))
			{
				ModelState.AddModelError("", "Email not activated, please check your email");
				return View(model);
			}

			if (await UserManager.IsLockedOutAsync(user.Id))
			{
				ModelState.AddModelError("", "Account locked");
				return View(model);
			}

			if (!await UserManager.CheckPasswordAsync(user, model.Password))
			{
				await IncrementAccessFailedCount(user, "Incorrect Email or Password");
				return View(model);
			}


			// Reset failed attempts
			await UserManager.ResetAccessFailedCountAsync(user.Id);

			await SignInAsync(user);
			if (string.IsNullOrEmpty(returnUrl))
			{
				return RedirectToAction("Index", "Home");
			}
			return RedirectToLocal(returnUrl);
		}


		private async Task SignInAsync(ApplicationUser user)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			var identity = await user.GenerateUserIdentityAsync(UserManager);
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
		}

		private async Task IncrementAccessFailedCount(ApplicationUser user, string message)
		{
			await UserManager.AccessFailedAsync(user.Id);
			if (await UserManager.IsLockedOutAsync(user.Id))
			{
				ModelState.AddModelError("", "Account locked");
				return;
			}
			ModelState.AddModelError("", message);
		}


		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (!CaptchaHelper.Validate())
			{
				ModelState.AddModelError("", "Invalid Captcha");
				return View(model);
			}

			if (ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					UserName = model.UserName,
					Email = model.Email,
					EmailConfirmed = false,
					Points = 0
				};

				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await AwardWriter.AddUserAward(new AddUserAwardModel
					{
						UserId = user.Id,
						Type = AwardType.Registration,
					});

					var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action(nameof(ConfirmEmailAddress), "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					await EmailService.SendEmail(Enums.EmailTemplateType.Registration, user.Id, user.Email, user.UserName, callbackUrl);
					return RedirectToAction(nameof(ConfirmEmail));
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult ConfirmEmail()
		{
			return View();
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmailAddress(int userId, string code)
		{
			if (code == null)
				return View("Error");

			var result = await UserManager.ConfirmEmailAsync(userId, code);
			if (result.Succeeded)
			{
				await SignInAsync(await UserManager.FindByIdAsync(userId));
				return RedirectToAction(nameof(ConfirmEmailSuccess));
			}

			return View("Error");
		}

		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult ConfirmEmailSuccess()
		{
			return View();
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!CaptchaHelper.Validate())
			{
				ModelState.AddModelError("", "Invalid Captcha");
				return View(model);
			}

			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByEmailAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return RedirectToAction(nameof(ForgotPasswordConfirmation));
				}

				string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				await EmailService.SendEmail(Enums.EmailTemplateType.PasswordReset, user.Id, user.Email, user.UserName, callbackUrl);
				return RedirectToAction(nameof(ForgotPasswordConfirmation));
			}

			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			if (string.IsNullOrEmpty(code))
				return View("Error");

			return View(new ResetPasswordViewModel
			{
				Code = code
			});
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!CaptchaHelper.Validate())
			{
				ModelState.AddModelError("", "Invalid Captcha");
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}

			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}



		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}




		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}
			}
			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
					: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}