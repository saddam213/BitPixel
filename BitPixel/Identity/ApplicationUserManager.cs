using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BitPixel.Base;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace BitPixel.Identity
{
	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
	public class ApplicationUserManager : UserManager<ApplicationUser, int>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
				: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};
			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("BitPixel.Identity"));
			}
			return manager;
		}


		internal const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
		internal const string DefaultIdentityProviderClaimValue = "BitPixel.Identity";

		public Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user)
		{
			return CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
		}

		public override Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			var userIdentity = new ClaimsIdentity(authenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String));
			userIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName, ClaimValueTypes.String));
			userIdentity.AddClaim(new Claim(IdentityProviderClaimType, DefaultIdentityProviderClaimValue, ClaimValueTypes.String));
			userIdentity.AddClaim(new Claim(Constants.DefaultSecurityStampClaimType, user.SecurityStamp));
			if (!user.Roles.IsNullOrEmpty())
			{
				foreach (var role in user.Roles)
				{
					userIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Role.Name, ClaimValueTypes.String));
				}
			}

			//// Custom claims
			//userIdentity.AddClaim(new Claim(ProjectXClaimTypes.ClientType, RequestClientType.WebV1.ToString()));
			//userIdentity.AddClaim(new Claim(ProjectXClaimTypes.TwoFactorType, user.TwoFactorType.ToString()));

			return Task.FromResult(userIdentity);
		}

	}

}
