using System;
using System.Data.Entity;
using System.Threading.Tasks;

using BitPixel.Common.DataContext;
using BitPixel.Common.Results;
using BitPixel.Common.Users;

namespace BitPixel.Core.Users
{
	public class UserWriter : IUserWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> UpdateUser(UpdateUserModal model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (user == null)
					return new WriterResult(false, "User not found");

				user.UserName = model.UserName;
				user.Email = model.Email;
				user.EmailConfirmed = model.IsEmailConfirmed;
				if(model.IsLocked && !user.LockoutEndDateUtc.HasValue)
				{
					user.LockoutEndDateUtc = DateTime.UtcNow.AddYears(1);
					user.SecurityStamp = Guid.NewGuid().ToString();
				}

				if (!model.IsLocked && user.LockoutEndDateUtc.HasValue)
				{
					user.LockoutEndDateUtc = null;
				}

				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}
	}
}
