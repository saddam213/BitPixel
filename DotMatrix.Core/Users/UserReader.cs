using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Users;
using DotMatrix.Entity;

namespace DotMatrix.Core.Users
{
	public class UserReader : IUserReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<UserModel> GetUser(int userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Users
				.Select(MapUser())
				.FirstOrDefaultAsync(x => x.Id == userId);
			}
		}



		public async Task<List<UserModel>> GetUsers()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Users
				.Select(MapUser())
				.ToListAsync();
			}
		}

		private Expression<Func<User, UserModel>> MapUser()
		{
			return x => new UserModel
			{
				Id = x.Id,
				UserName = x.UserName,
				Email = x.Email,
				ApiKey = x.ApiKey,
				ApiSecret = x.ApiSecret,
				IsApiEnabled = x.IsApiEnabled,
				TeamId = x.TeamId,
				Points = x.Points,
				Team = x.Team.Name,
				IsEmailConfirmed = x.EmailConfirmed,
				IsLocked = x.LockoutEndDateUtc.HasValue
			};
		}


		public async Task<UserProfileModel> GetUserProfile(string userName)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Users
				.Where(x => x.UserName == userName)
				.Select(x => new UserProfileModel
				{
					Id = x.Id,
					UserName = x.UserName,
					Clicks = x.Clicks.Count,
					Pixels = x.Pixels.Count,
					Awards = x.Awards.Count
				}).FirstOrDefaultAsync();
			}
		}
	}
}
