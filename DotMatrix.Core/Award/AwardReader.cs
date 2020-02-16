using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using DotMatrix.Common.Award;
using DotMatrix.Common.DataContext;
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;
using DotMatrix.Enums;

namespace DotMatrix.Core.Award
{
	public class AwardReader : IAwardReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<AwardModel> GetAward(int awardId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Award
					.Where(x => x.Id == awardId)
					.Select(x => new AwardModel
					{
						Id = x.Id,
						Icon = x.Icon,
						Name = x.Name,
						Level = x.Level,
						Description = x.Description,
						Points = x.Points,
						Timestamp = x.Timestamp,
						TriggerType = x.TriggerType,
						ClickType = x.ClickType,
						Rank = x.Rank,
						Status = x.Status
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<List<AwardModel>> GetAwards()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Award
					.Select(x => new AwardModel
					{
						Id = x.Id,
						Icon = x.Icon,
						Name = x.Name,
						Level = x.Level,
						Description = x.Description,
						Points = x.Points,
						Timestamp = x.Timestamp,
						TriggerType = x.TriggerType,
						ClickType = x.ClickType,
						Rank = x.Rank,
						Status = x.Status
					}).ToListAsync();
			}
		}

		public async Task<AwardUserModel> GetUserAward(int userId, int awardId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Where(x => x.UserId == userId && x.AwardId == awardId)
					.GroupBy(x => x.Award)
					.Select(x => new AwardUserModel
					{
						Id = x.Key.Id,
						Points = x.Key.Points,
						Icon = x.Key.Icon,
						Name = x.Key.Name,
						Level = x.Key.Level,
						Description = x.Key.Description,
						TriggerType = x.Key.TriggerType,
						ClickType = x.Key.ClickType,
						Count = x.Count()
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<List<AwardListItemModel>> GetAwardList()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Award
					.Where(x => x.Status == AwardStatus.Active)
					.Select(x => new AwardListItemModel
					{
						AwardId = x.Id,
						Icon = x.Icon,
						Name = x.Name,
						Level = x.Level,
						ClickType = x.ClickType,
						Points = x.Points,
						Rank = x.Rank
					})
					.OrderBy(x => x.Name)
					.ToListAsync();
			}
		}

		public async Task<List<AwardUserListItemModel>> GetUserAwardList(int userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Where(x => x.UserId == userId)
					.GroupBy(x => x.Award)
					.Select(x => new AwardUserListItemModel
					{
						AwardId = x.Key.Id,
						Icon = x.Key.Icon,
						Name = x.Key.Name,
						Level = x.Key.Level,
						ClickType = x.Key.ClickType,
						Rank = x.Key.Rank,
						Count = x.Count()
					})
					.OrderBy(x => x.Name)
					.ToListAsync();
			}
		}

		public async Task<DataTablesResponseData> GetHistory(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Select(x => new
					{
						Id = x.Id,
						AwardId = x.AwardId,
						GameId = x.GameId,
						Icon = x.Award.Icon,
						Name = x.Award.Name,
						Level = x.Award.Level,
						ClickType = x.Award.ClickType,
						Points = x.Points,
						Version = x.VersionData,
						Game = x.Game == null ? string.Empty : x.Game.Name,
						Player = x.User.UserName,
						Timestamp = x.Timestamp
					}).GetDataTableResponseAsync(model);
			}
		}

		public async Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Where(x => x.UserId == userId)
					.Select(x => new
					{
						Id = x.Id,
						AwardId = x.AwardId,
						GameId = x.GameId,
						Icon = x.Award.Icon,
						Name = x.Award.Name,
						Level = x.Award.Level,
						ClickType = x.Award.ClickType,
						Points = x.Points,
						Version = x.VersionData,
						Game = x.GameId.HasValue ? x.Game.Name : string.Empty,
						Timestamp = x.Timestamp
					}).GetDataTableResponseAsync(model);
			}
		}

		public async Task<DataTablesResponseData> GetGameHistory(DataTablesParam model, int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.AwardHistory
					.Where(x => x.GameId == gameId)
					.Select(x => new
					{
						Id = x.Id,
						AwardId = x.AwardId,
						Icon = x.Award.Icon,
						Name = x.Award.Name,
						Level = x.Award.Level,
						ClickType = x.Award.ClickType,
						Points = x.Points,
						Version = x.VersionData,
						Player = x.User.UserName,
						Timestamp = x.Timestamp
					}).GetDataTableResponseAsync(model);
			}
		}

		public async Task<DataTablesResponseData> GetAwards(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Award
					.Where(x => x.Status == AwardStatus.Active)
					.Select(x => new
					{
						Id = x.Id,
						Icon = x.Icon,
						Name = x.Name,
						Level = x.Level,
						ClickType = x.ClickType,
						Points = x.Points,
						Timestamp = x.Timestamp
					}).GetDataTableResponseAsync(model);
			}
		}
	}
}
