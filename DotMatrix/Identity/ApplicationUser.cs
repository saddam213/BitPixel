using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Identity
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IUser<int>
	{
		public ApplicationUser()
		{
			Roles = new List<ApplicationUserRole>();
		}

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[MaxLength(128)]
		public string UserName { get; set; }

		[MaxLength(256)]
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }

		[MaxLength(256)]
		public string PasswordHash { get; set; }

		[MaxLength(50)]
		public string SecurityStamp { get; set; }
		public DateTime? LockoutEndDateUtc { get; set; }
		public bool LockoutEnabled { get; set; }
		public int AccessFailedCount { get; set; }


		public int TeamId { get; set; }
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public bool IsApiEnabled { get; set; }
		public int Points { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}

		public virtual ICollection<ApplicationUserRole> Roles { get; set; }
	}
}