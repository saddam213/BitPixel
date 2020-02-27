using System;

using BitPixel.Hubs;
using BitPixel.Identity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

using Owin;

namespace BitPixel
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
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, int>(
									validateInterval: TimeSpan.FromSeconds(30),
									regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
									 getUserIdCallback: (id) => id.GetUserId<int>())
				},
				CookieName = "bitpixel"
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
		
			var identityProvider = new HubIdentityProvider();
			GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => identityProvider);
			app.MapSignalR(hubConfiguration);
		}
	}
}