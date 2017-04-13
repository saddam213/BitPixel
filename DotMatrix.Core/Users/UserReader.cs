using DotMatrix.Common.DataContext;
using DotMatrix.Common.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Entity;
using System.Data.Entity;

namespace DotMatrix.Core.Users
{
	public class UserReader : IUserReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<User>> GetUsers()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Users.ToListAsync();
			}
		}
	}
}
