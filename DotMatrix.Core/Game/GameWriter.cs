using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using DotMatrix.Cache.Common;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Game;
using DotMatrix.Common.Results;

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
	}
}
