using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotMatrix.Cache.Common
{
	public interface IGameCache
	{
		Task<GameCacheItem> GetGame(int gameId);
		Task<IEnumerable<GameCacheItem>> GetGames();
	}
}
