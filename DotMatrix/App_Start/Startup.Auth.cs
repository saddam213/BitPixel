using System;

using DotMatrix.Hubs;
using DotMatrix.Identity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

using Owin;

namespace DotMatrix
{
	public partial class Startup
	{
		public void ConfigureAuth(IAppBuilder app)
		{
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login"),
				Provider = new CookieAuthenticationProvider
				{
					// Enables the application to validate the security stamp when the user logs in.
					// This is a security feature which is used when you change a password or add an external login to your account.  
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, int>(
									validateInterval: TimeSpan.FromMinutes(30),
									regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
									 getUserIdCallback: (id) => id.GetUserId<int>())
				}
			});

			app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

			// Signalr
			var hubConfiguration = new HubConfiguration
			{
#if DEBUG
				EnableDetailedErrors = true,
#else
				EnableDetailedErrors = false
#endif
			};
			GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new HubIdentityProvider());
			app.MapSignalR(hubConfiguration);
		}
	}
}