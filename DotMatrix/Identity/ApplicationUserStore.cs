using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer.Utilities;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Identity
{
	public class ApplicationUserStore :
				IQueryableUserStore<ApplicationUser, int>,
				IUserStore<ApplicationUser, int>,
				IUserPasswordStore<ApplicationUser, int>,
				IUserSecurityStampStore<ApplicationUser, int>,
				IUserEmailStore<ApplicationUser, int>,
				IUserLockoutStore<ApplicationUser, int>
	{
		private bool _disposed;
		private ApplicationEntityStore<ApplicationUser> _userStore;

		public ApplicationUserStore(ApplicationDbContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			Context = context;
			_userStore = new ApplicationEntityStore<ApplicationUser>(context);
		}

		public bool DisposeContext { get; set; }
		public ApplicationDbContext Context { get; private set; }


		public IQueryable<ApplicationUser> Users
		{
			get { return _userStore.EntitySet; }
		}

		public async Task CreateAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			_userStore.Create(user);
			await SaveChanges().WithCurrentCulture();
		}

		public async Task DeleteAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			_userStore.Delete(user);
			await SaveChanges().WithCurrentCulture();
		}

		public async Task UpdateAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			_userStore.Update(user);
			await SaveChanges().WithCurrentCulture();
		}

		public Task<ApplicationUser> FindByIdAsync(int userId)
		{
			ThrowIfDisposed();
			return Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
		}

		public Task<ApplicationUser> FindByNameAsync(string userName)
		{
			ThrowIfDisposed();
			return Users.FirstOrDefaultAsync(u => u.UserName == userName);
		}

		#region IUserEmailStore

		public Task<ApplicationUser> FindByEmailAsync(string email)
		{
			ThrowIfDisposed();
			return Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.EmailConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public Task SetEmailAsync(ApplicationUser user, string email)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.Email = email;
			return Task.FromResult(0);
		}

		public Task<string> GetEmailAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.Email);
		}

		#endregion


		#region IUserPasswordStore

		public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}

		public Task<string> GetPasswordHashAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(ApplicationUser user)
		{
			return Task.FromResult(user.PasswordHash != null);
		}

		#endregion


		#region IUserLockoutStore


		public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return
					Task.FromResult(user.LockoutEndDateUtc.HasValue
							? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
							: new DateTimeOffset());
		}

		public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
			return Task.FromResult(0);
		}

		public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.AccessFailedCount++;
			return Task.FromResult(user.AccessFailedCount);
		}

		public Task ResetAccessFailedCountAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.AccessFailedCount = 0;
			return Task.FromResult(0);
		}

		public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.AccessFailedCount);
		}

		public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.LockoutEnabled);
		}


		public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.LockoutEnabled = enabled;
			return Task.FromResult(0);
		}

		#endregion


		#region IUserSecurityStampStore


		public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}

		public Task<string> GetSecurityStampAsync(ApplicationUser user)
		{
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.SecurityStamp);
		}

		#endregion










		// Only call save changes if AutoSaveChanges is true
		private async Task SaveChanges()
		{
			await Context.SaveChangesAsync().WithCurrentCulture();
		}


		/// <summary>
		///     Dispose the store
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}



		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		///     If disposing, calls dispose on the Context.  Always nulls out the Context
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (DisposeContext && disposing && Context != null)
			{
				Context.Dispose();
			}
			_disposed = true;
			Context = null;
			_userStore = null;
		}



		private class ApplicationEntityStore<TEntity> where TEntity : class
		{
			/// <summary>
			///     Constructor that takes a Context
			/// </summary>
			/// <param name="context"></param>
			public ApplicationEntityStore(ApplicationDbContext context)
			{
				Context = context;
				DbEntitySet = context.Set<TEntity>();
			}

			/// <summary>
			///     Context for the store
			/// </summary>
			public ApplicationDbContext Context { get; private set; }

			/// <summary>
			///     Used to query the entities
			/// </summary>
			public IQueryable<TEntity> EntitySet
			{
				get { return DbEntitySet; }
			}

			/// <summary>
			///     EntitySet for this store
			/// </summary>
			public DbSet<TEntity> DbEntitySet { get; private set; }

			/// <summary>
			///     FindAsync an entity by ID
			/// </summary>
			/// <param name="id"></param>
			/// <returns></returns>
			public virtual Task<TEntity> GetByIdAsync(object id)
			{
				return DbEntitySet.FindAsync(id);
			}

			/// <summary>
			///     Insert an entity
			/// </summary>
			/// <param name="entity"></param>
			public void Create(TEntity entity)
			{
				DbEntitySet.Add(entity);
			}

			/// <summary>
			///     Mark an entity for deletion
			/// </summary>
			/// <param name="entity"></param>
			public void Delete(TEntity entity)
			{
				DbEntitySet.Remove(entity);
			}

			/// <summary>
			///     Update an entity
			/// </summary>
			/// <param name="entity"></param>
			public virtual void Update(TEntity entity)
			{
				if (entity != null)
				{
					Context.Entry(entity).State = EntityState.Modified;
				}
			}
		}
	}
}