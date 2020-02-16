using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using DotMatrix.Cache.Common;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Game;

namespace DotMatrix.Core.Game
{
	public class GameReader : IGameReader
	{
		public IGameCache GameCache { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<GameModel> GetGame(int gameId)
		{
			return MapGameModel(await GameCache.GetGame(gameId));
		}

		public async Task<List<GameModel>> GetGames()
		{
			var gameItems = await GameCache.GetGames();
			return gameItems
				.Select(MapGameModel)
				.ToList();
		}

		public async Task<List<GameModel>> GetGames(Enums.GameStatus status)
		{
			var gameItems = await GameCache.GetGames();
			return gameItems
				.Where(x => x.Status == status)
				.Select(MapGameModel)
				.ToList();
		}

		private static GameModel MapGameModel(GameCacheItem cacheItem)
		{
			if (cacheItem == null)
				return null;

			return new GameModel
			{
				Id = cacheItem.Id,
				Type = cacheItem.Type,
				Status = cacheItem.Status,
				Width = cacheItem.Width,
				Height = cacheItem.Height,
				Name = cacheItem.Name,
				Rank = cacheItem.Rank,
				Description = cacheItem.Description,
				Timestamp = cacheItem.Timestamp,
				ClicksPerSecond = cacheItem.ClicksPerSecond,
				Platform = cacheItem.Platform
	};
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
	}
}
