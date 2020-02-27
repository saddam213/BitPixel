using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using DotMatrix.Cache.Common;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Game;
using DotMatrix.Common.Results;
using DotMatrix.Common.Team;

namespace DotMatrix.Core.Game
{
	public class GameWriter : IGameWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateGame(CreateGameModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				if (await GameNameExists(context, model.Name))
					return new WriterResult(false, "Game with name already exists");

				context.Games.Add(new Entity.Game
				{
					Name = model.Name,
					Description = model.Description,
					Type = model.Type,
					Status = Enums.GameStatus.NotStarted,
					Platform = model.Platform,
					Width = model.Width,
					Height = model.Height,
					ClicksPerSecond = model.ClicksPerSecond,
					Rank = model.Rank,
					Timestamp = DateTime.UtcNow
				});
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}

		private static async Task<bool> GameNameExists(IDataContext context, string name)
		{
			return await context.Games
				.Where(x => x.Status != Enums.GameStatus.Deleted && x.Status != Enums.GameStatus.Finished)
				.AnyAsync(x => x.Name == name);
		}

		public async Task<IWriterResult> UpdateGame(UpdateGameModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var game = await context.Games.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (game == null)
					return new WriterResult(false, "Game not found");

				if (game.Status == Enums.GameStatus.Deleted)
					return new WriterResult(false, "Unable to update deleted game");

				// if name has changed, check for duplicates
				if (!game.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase))
				{
					if (await GameNameExists(context, model.Name))
						return new WriterResult(false, "Game with name already exists");

					game.Name = model.Name;
				}

				if (game.Status != model.Status)
				{
					if (game.Status == Enums.GameStatus.Finished && model.Status != Enums.GameStatus.Deleted)
						return new WriterResult(false, $"Finished game cannot be set to {model.Status} status");
				}

				game.Rank = model.Rank;
				game.Status = model.Status;
				game.Platform = model.Platform;
				game.Description = model.Description;
				game.ClicksPerSecond = model.ClicksPerSecond;

				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> CreateTeam(CreateTeamModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var game = await context.Games.FirstOrDefaultAsync(x => x.Id == model.GameId);
				if (game == null)
					return new WriterResult(false, $"Game not found");

				if (game.Status != Enums.GameStatus.NotStarted && game.Status != Enums.GameStatus.Paused)
					return new WriterResult(false, $"Unable to add team to game in {game.Status} status");

				if (await context.Team.AnyAsync(x => x.GameId == game.Id && x.Name == model.Name))
					return new WriterResult(false, $"Game with name already exists");

				if (await context.Team.AnyAsync(x => x.GameId == game.Id && x.Color == model.Color))
					return new WriterResult(false, $"Game with color already exists");

				context.Team.Add(new Entity.Team
				{
					GameId = game.Id,
					Name = model.Name,
					Description = model.Description,
					Icon = model.Icon,
					Color = model.Color,
					Rank = model.Rank,
					Timestamp = DateTime.UtcNow
				});
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> UpdateTeam(UpdateTeamModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var team = await context.Team
					.Include(x => x.Game)
					.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (team == null)
					return new WriterResult(false, $"Team not found");

				if (team.Game.Status != Enums.GameStatus.NotStarted && team.Game.Status != Enums.GameStatus.Paused)
					return new WriterResult(false, $"Unable to update team to game in {team.Game.Status} status");

				if (await context.Team.AnyAsync(x => x.GameId == team.GameId && x.Name == model.Name && x.Id != model.Id))
					return new WriterResult(false, $"Game with name already exists");

				if (await context.Team.AnyAsync(x => x.GameId == team.GameId && x.Color == model.Color && x.Id != model.Id))
					return new WriterResult(false, $"Game with color already exists");

				team.Name = model.Name;
				team.Description = model.Description;
				team.Color = model.Color;
				team.Icon = model.Icon;
				team.Rank = model.Rank;
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> ChangeTeam(int userId, ChangeTeamModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var team = await context.Team.FirstOrDefaultAsync(x => x.Id == model.TeamId && x.GameId == model.GameId);
				if (team == null)
					return new WriterResult(false, $"Team not found");

				var teamMember = await context.TeamMember.FirstOrDefaultAsync(x => x.UserId == userId && x.Team.GameId == model.GameId);
				if (teamMember != null)
				{
					if (teamMember.TeamId == model.TeamId)
						return new WriterResult(false, $"Already member of team {teamMember.Team.Name}");

					teamMember.TeamId = team.Id;
					await context.SaveChangesAsync();
					return new WriterResult(true);
				}

				context.TeamMember.Add(new Entity.TeamMember
				{
					UserId = userId,
					TeamId = team.Id,
					Timestamp = DateTime.UtcNow
				});
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}
	}
}
