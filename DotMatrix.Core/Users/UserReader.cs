using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Users;
using DotMatrix.Entity;

namespace DotMatrix.Core.Users
{
	public class UserReader : IUserReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<User> GetUser(int userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
			}
		}

		public async Task<List<User>> GetUsers()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Users.ToListAsync();
			}
		}
	}
}
