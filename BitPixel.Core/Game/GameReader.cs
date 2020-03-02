using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using BitPixel.Common.DataContext;
using BitPixel.Common.Game;
using BitPixel.Common.Prize;
using BitPixel.Common.Team;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;

namespace BitPixel.Core.Game
{
	public class GameReader : IGameReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<GameModel> GetGame(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Games
					.Where(x => x.Id == gameId && x.Status != Enums.GameStatus.Deleted)
					.Select(x => new GameModel
					{
						Id = x.Id,
						Type = x.Type,
						Status = x.Status,
						Width = x.Width,
						Height = x.Height,
						Name = x.Name,
						Rank = x.Rank,
						Description = x.Description,
						Timestamp = x.Timestamp,
						ClicksPerSecond = x.ClicksPerSecond,
						Platform = x.Platform,
						EndType = x.EndType,
						EndTime = x.EndTime
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<List<GameModel>> GetGames()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Games
					.Where(x => x.Status != Enums.GameStatus.Deleted)
					.Select(x => new GameModel
					{
						Id = x.Id,
						Type = x.Type,
						Status = x.Status,
						Width = x.Width,
						Height = x.Height,
						Name = x.Name,
						Rank = x.Rank,
						Description = x.Description,
						Timestamp = x.Timestamp,
						ClicksPerSecond = x.ClicksPerSecond,
						Platform = x.Platform,
						EndType = x.EndType,
						EndTime = x.EndTime
					}).ToListAsync();
			}
		}

		public async Task<List<string>> GetPlayers(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.PixelHistory
					.Where(x => x.GameId == gameId)
					.Select(x => x.User.UserName)
					.Distinct()
					.OrderBy(x => x)
					.ToListAsync();
			}
		}

		public async Task<TeamModel> GetTeam(int teamId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Team
					.Where(x => x.Id == teamId)
					.Select(x => new TeamModel
					{
						Id = x.Id,
						GameId = x.GameId,
						Game = x.Game.Name,
						Name = x.Name,
						Description = x.Description,
						Icon = x.Icon,
						Color = x.Color,
						Rank = x.Rank,
						Result = x.Result
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<List<TeamModel>> GetTeams(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Team
					.Where(x => x.GameId == gameId)
					.Select(x => new TeamModel
					{
						Id = x.Id,
						GameId = x.GameId,
						Game = x.Game.Name,
						Name = x.Name,
						Description = x.Description,
						Icon = x.Icon,
						Color = x.Color,
						Rank = x.Rank,
						Result = x.Result
					}).ToListAsync();
			}
		}

		public async Task<DataTablesResponseData> GetTeams(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Team
					.Select(x => new
					{
						Id = x.Id,
						GameId = x.GameId,
						Game = x.Game.Name,
						Name = x.Name,
						Description = x.Description,
						Icon = x.Icon,
						Color = x.Color,
						Members = x.Members.Count,
						Rank = x.Rank
					}).GetDataTableResponseAsync(model);
			}
		}

		public async Task<TeamModel> GetUserTeam(int userId, int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.TeamMember
					.Where(x => x.UserId == userId && x.Team.GameId == gameId)
					.Select(x => new TeamModel
					{
						Id = x.Team.Id,
						GameId = x.Team.GameId,
						Game = x.Team.Game.Name,
						Name = x.Team.Name,
						Description = x.Team.Description,
						Icon = x.Team.Icon,
						Color = x.Team.Color,
						Rank = x.Team.Rank,
						Result = x.Team.Result,
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<GameStatsModel> GetStats(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Games
					.Include(x => x.Teams)
					.Include(x => x.Pixels)
					.Include(x => x.Prizes)
					.Where(x => x.Id == gameId)
					.Select(x => new GameStatsModel
					{
						Width = x.Width,
						Height = x.Height,
						FixedPixels = x.Pixels
							.Count(p => p.Type == Enums.PixelType.Fixed),
						UserPixels = x.Pixels
							.Count(p => p.Type == Enums.PixelType.User),

						Prizes = x.Prizes.Count,
						PrizesFound = x.Prizes
							.Count(p => p.IsClaimed),

						TeamStats = x.Teams
							.Select(p => new TeamStatsModel
							{
								TeamId = p.Id,
								Name = p.Name,
								Icon = p.Icon,
								Color = p.Color,
								Members = p.Members.Count,
								Pixels = p.Pixels.Count,
								Rank = p.Rank
							})
							.OrderByDescending(p => p.Pixels)
							.ThenBy(p => p.Rank)
							.ToList()
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<GamePrizeViewModel> GetPrizes(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Games
				.Where(x => x.Id == gameId)
				.Select(MapGamePrizes())
				.FirstOrDefaultAsync();
			}
		}



		public async Task<List<GamePrizeViewModel>> GetPrizes()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Games
				.Where(x => x.Status != Enums.GameStatus.Finished && x.Status != Enums.GameStatus.Deleted)
				.Select(MapGamePrizes())
				.ToListAsync();
			}
		}

		public async Task<DataTablesResponseData> GetPixelPrizes(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Prize
				.Where(x => x.Game.Status != Enums.GameStatus.Finished && x.Game.Status != Enums.GameStatus.Deleted)
				.GroupBy(x => new
				{
					GameId = x.GameId,
					Game = x.Game.Name,
					Name = x.Name,
					Description = x.Description,
					Type = x.Type
				})
				.Select(x => new
				{
					GameId = x.Key.GameId,
					Game = x.Key.Game,
					Name = x.Key.Name,
					Description = x.Key.Description,
					Type = x.Key.Type,
					Count = x.Count(),
					Remaining = x.Count(p => p.IsClaimed)
				})
				.GetDataTableResponseAsync(model);
			}
		}


		public async Task<DataTablesResponseData> GetGamePrizes(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.GamePrize
				.Where(x => x.Game.Status != Enums.GameStatus.Finished && x.Game.Status != Enums.GameStatus.Deleted)
				.Select(x => new
				{
					GameId = x.GameId,
					Game = x.Game.Name,
					Name = x.Name,
					Description = x.Description,
					Type = x.Type
				}).GetDataTableResponseAsync(model);
			}
		}


		private static Expression<Func<Entity.Game, GamePrizeViewModel>> MapGamePrizes()
		{
			return game => new GamePrizeViewModel
			{
				GameId = game.Id,
				Rank = game.Rank,
				Name = game.Name,
				Description = game.Description,
				EndType = game.EndType,
				EndTime = game.EndTime,
				Status = game.Status,
				GamePrizes = game.GamePrizes
					.Select(x => new GamePrizeModel
					{
						Id = x.Id,
						GameId = x.GameId,
						Name = x.Name,
						Description = x.Description,
						Points = x.Points,
						Type = x.Type,
						Rank = x.Rank,
						Symbol = x.Data,
					}).ToList(),
				PixelPrizes = game.Prizes
					.GroupBy(x => new
					{
						Name = x.Name,
						Description = x.Description,
						Type = x.Type,
						Game = x.Game.Name,
						GameId = x.GameId,
						GameRank = x.Game.Rank,
						Symbol = x.Data,
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
					}).ToList()
			};
		}


	}
}

