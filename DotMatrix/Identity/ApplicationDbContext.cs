using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace DotMatrix.Identity
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext()
				: base("DefaultConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public virtual IDbSet<ApplicationUser> Users { get; set; }
		public virtual IDbSet<ApplicationRole> Roles { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//base.OnModelCreating(modelBuilder);
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(22, 8));

			modelBuilder.Entity<ApplicationUser>()
				.ToTable("Users")
				.Property(u => u.UserName)
					.IsRequired()
					.HasMaxLength(256)
					.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

			modelBuilder.Entity<ApplicationRole>()
			.ToTable("UserRoles")
				.Property(r => r.Name)
					.IsRequired()
					.HasMaxLength(256)
					.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));

			modelBuilder.Entity<ApplicationUserRole>()
				.ToTable("UserInRoles")
				.HasKey(r => new { r.RoleId, r.UserId });//.HasRequired(p => p.User);

			modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(128);
			modelBuilder.Entity<ApplicationUser>().Property(u => u.UserName).HasMaxLength(50);
			modelBuilder.Entity<ApplicationUser>().Property(u => u.PasswordHash).HasMaxLength(256);
			modelBuilder.Entity<ApplicationUser>().Property(u => u.SecurityStamp).HasMaxLength(50);
		}

		protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
		{
			if (entityEntry != null && entityEntry.State == EntityState.Added)
			{
				var errors = new List<DbValidationError>();
				if (entityEntry.Entity is ApplicationUser user)
				{
					if (Users.Any(u => string.Equals(u.UserName, user.UserName)))
					{
						errors.Add(new DbValidationError("User", string.Format(CultureInfo.CurrentCulture, "IdentityResources.DuplicateUserName", user.UserName)));
					}
				}
				else
				{
					var role = entityEntry.Entity as ApplicationRole;
					//check for uniqueness of role name
					if (role != null && Roles.Any(r => string.Equals(r.Name, role.Name)))
					{
						errors.Add(new DbValidationError("Role", string.Format(CultureInfo.CurrentCulture, "IdentityResources.RoleAlreadyExists", role.Name)));
					}
				}
				if (errors.Any())
				{
					return new DbEntityValidationResult(entityEntry, errors);
				}
			}
			return base.ValidateEntity(entityEntry, items);
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
	}
}