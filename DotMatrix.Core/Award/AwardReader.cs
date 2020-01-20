using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Award;
using DotMatrix.Enums;

namespace DotMatrix.Core.Award
{
	public class AwardReader : IAwardReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<AwardItemModel>> GetAwards()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Award
				.Where(x => x.Status == AwardStatus.Active)
				.Select(x => new AwardItemModel
				{
					Name = x.Name,
					Points = x.Points
				}).ToListAsync();
			}
		}

		public async Task<List<AwardHistoryItemModel>> GetAwardHistory()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Select(x => new AwardHistoryItemModel
					{
						Name = x.Award.Name,
						Points = x.Points,
						Timestamp = x.Timestamp,
						UserName = x.User.UserName
					}).ToListAsync();
			}
		}

		public async Task<List<AwardUserHistoryItemModel>> GetUserAwards(int userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Where(x => x.UserId == userId)
					.Select(x => new AwardUserHistoryItemModel
					{
						Id = x.Id,
						Name = x.Award.Name,
						Points = x.Points,
						Timestamp = x.Timestamp
					})
					.OrderByDescending(x => x.Timestamp)
					.ToListAsync();
			}
		}

		public async Task<AwardUserHistoryItemModel> GetUserAward(int userId, int awardId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return new AwardUserHistoryItemModel();
			}
		}

	}
}
