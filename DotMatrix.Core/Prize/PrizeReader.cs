using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Prize;
using DotMatrix.Enums;

namespace DotMatrix.Core.Prize
{
	public class PrizeReader : IPrizeReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<PrizeItemModel>> GetPrizes()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.GroupBy(x => new
					{
						x.Name,
						x.Description,
						x.Type
					})
					.Select(x => new PrizeItemModel
					{
						Name = x.Key.Name,
						Description = x.Key.Description,
						Type = x.Key.Type,

						Count = x.Count(),
						Unclaimed = x.Count(p => !p.IsClaimed)
					})
					.OrderByDescending(x => x.Unclaimed)
					.ToListAsync();
			}
		}

		public async Task<List<PrizeHistoryItemModel>> GetPrizeHistory()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(x => x.IsClaimed)
					.Select(x => new PrizeHistoryItemModel
					{
						X = x.X,
						Y = x.Y,
						UserName = x.User.UserName,
						Type = x.Type,
						Name = x.Name,
						Description = x.Description,
						Points = x.Points,
						Timestamp = x.Timestamp
					})
					.OrderByDescending(x => x.Timestamp)
					.ToListAsync();
			}
		}


		public async Task<List<PrizeUserHistoryItemModel>> GetUserPrizes(int userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.IsClaimed && p.UserId == userId)
					.Select(MapUserPrize())
					.ToListAsync();
			}
		}

		public async Task<PrizeUserHistoryItemModel> GetUserPrize(int userId, int prizeId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.Id == prizeId && p.IsClaimed && p.UserId == userId)
					.Select(MapUserPrize())
					.FirstOrDefaultAsync();
			}
		}

		private static Expression<Func<Entity.Prize, PrizeUserHistoryItemModel>> MapUserPrize()
		{
			return p => new PrizeUserHistoryItemModel
			{
				Id = p.Id,
				X = p.X,
				Y = p.Y,
				Name = p.Name,
				Description = p.Description,
				Points = p.Points,
				Timestamp = p.ClaimTime ?? p.Timestamp,
				Type = p.Type
			};
		}
	}
}
