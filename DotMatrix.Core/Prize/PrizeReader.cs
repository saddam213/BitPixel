using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Prize;
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;

namespace DotMatrix.Core.Prize
{
	public class PrizeReader : IPrizeReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<PrizeItemModel>> GetPrizes(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await MapPrizes(context.Prize.Where(x => x.GameId == gameId))
					.OrderByDescending(x => x.Unclaimed)
					.ToListAsync();
			}
		}

		public async Task<List<PrizeUserHistoryItemModel>> GetPrizePayments()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.IsClaimed && p.Type == Enums.PrizeType.Crypto)
					.Select(MapUserPrize())
					.ToListAsync();
			}
		}


		public async Task<DataTablesResponseData> GetPrizePayments(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.IsClaimed && p.Type == Enums.PrizeType.Crypto)
					.Select(x => new
					{
						x.Id,
						Game = x.Game.Name,
						x.Name,
						x.User.UserName,
						x.Data,
						x.Data2,
						x.Data4,
						x.Status,
						x.ClaimTime,
					}).GetDataTableResponseAsync(model);
			}
		}


		public async Task<PrizeUserHistoryItemModel> GetPrizePayment(int prizeId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.IsClaimed && p.Id == prizeId && p.Type == Enums.PrizeType.Crypto)
					.Select(MapUserPrize())
					.FirstOrDefaultAsync();
			}
		}

		public async Task<List<PrizeItemModel>> GetPrizes()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await MapPrizes(context.Prize)
					.OrderBy(x => x.Game)
					.ThenBy(x => x.Name)
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

		private static Expression<Func<Entity.Prize, PrizeHistoryItemModel>> MapPrize()
		{
			return x => new PrizeHistoryItemModel
			{
				X = x.X,
				Y = x.Y,
				UserName = x.User.UserName,
				Type = x.Type,
				Name = x.Name,
				Game = x.Game.Name,
				GameId = x.GameId,
				Description = x.Description,
				Points = x.Points,
				Timestamp = x.ClaimTime ?? x.Timestamp
			};
		}

		private static Expression<Func<Entity.Prize, PrizeUserHistoryItemModel>> MapUserPrize()
		{
			return p => new PrizeUserHistoryItemModel
			{
				Id = p.Id,
				X = p.X,
				Y = p.Y,
				UserName = p.User.UserName,
				Name = p.Name,
				Game = p.Game.Name,
				GameId = p.GameId,
				Description = p.Description,
				Points = p.Points,
				Timestamp = p.ClaimTime ?? p.Timestamp,
				Type = p.Type,
				Status = p.Status,
				Data = p.Data,
				Data2 = p.Data2,
				Data3 = p.Data3,
				Data4 = p.Data4
			};
		}

		private static IQueryable<PrizeItemModel> MapPrizes(IQueryable<Entity.Prize> query)
		{
			return query.GroupBy(x => new
			{
				x.Name,
				x.Description,
				x.Type,
				Game = x.Game.Name,
				x.GameId,
				GameRank = x.Game.Rank,
				Symbol = x.Data
			})
			.Select(x => new PrizeItemModel
			{
				Name = x.Key.Name,
				Description = x.Key.Description,
				Type = x.Key.Type,
				Game = x.Key.Game,
				GameId = x.Key.GameId,
				GameRank = x.Key.GameRank,
				Symbol = x.Key.Symbol,
				Count = x.Count(),
				Unclaimed = x.Count(p => !p.IsClaimed)
			});
		}





		public async Task<DataTablesResponseData> GetHistory(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.IsClaimed && p.Game.Status != Enums.GameStatus.Deleted)
					.Select(x => new
					{
						Id = x.Id,
						GameId = x.GameId,
						Game = x.Game.Name,
						Name = x.Name,
						Description = x.Description,
						X = x.X,
						Y = x.Y,
						Points = x.Points,
						UserName = x.User.UserName,
						Timestamp = x.ClaimTime,
					}).GetDataTableResponseAsync(model);
			}
		}


		public async Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? count)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Prize
					.Where(p => p.IsClaimed && p.UserId == userId)
					.Select(x => new
					{
						Id = x.Id,
						GameId = x.GameId,
						IsUnclaimed = x.Status == Enums.PrizeStatus.Unclaimed,
						Game = x.Game.Name,
						Name = x.Name,
						Description = x.Description,
						X = x.X,
						Y = x.Y,
						Points = x.Points,
						Status = x.Status,
						Timestamp = x.ClaimTime,
					});
				if (count.HasValue)
				{
					query = query
						.OrderBy(x => x.Status)
						.ThenByDescending(x => x.Timestamp)
						.Take(count.Value);
				}
				return await query.GetDataTableResponseAsync(model, disablePaging: count.HasValue);
			}
		}

		public async Task<DataTablesResponseData> GetGameHistory(DataTablesParam model, int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(p => p.IsClaimed && p.GameId == gameId)
					.Select(x => new
					{
						Id = x.Id,
						Name = x.Name,
						Description = x.Description,
						X = x.X,
						Y = x.Y,
						Points = x.Points,
						UserName = x.User.UserName,
						Timestamp = x.ClaimTime,
					}).GetDataTableResponseAsync(model);
			}
		}

		public async Task<PrizeHistoryItemModel> GetPrize(int prizeId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
					.Where(x => x.IsClaimed && x.Id == prizeId)
					.Select(MapPrize())
					.FirstOrDefaultAsync();
			}
		}


	}
}
