using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitPixel.Cache.Common
{
	public interface IGameCache
	{
		Task<GameCacheItem> GetGame(int gameId);
		Task<IEnumerable<GameCacheItem>> GetGames();
	}
}
