using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

using BitPixel.Cache.Common;
using BitPixel.Common.DataContext;

using StackExchange.Redis;

namespace BitPixel.Cache
{
	public class GameCache : DynamicTableCache<GameCacheItem, int>, IGameCache
	{
		private readonly TimeSpan _expireTimeConfig = TimeSpan.FromMilliseconds(5000);

		public static async Task<IGameCache> BuildCache(string nodeKey)
		{
			var cache = new GameCache(nodeKey);
			await cache.Initialize();
			return cache;
		}

		public GameCache(string nodeKey)
			: base(nodeKey) { }
	
		public async Task<GameCacheItem> GetGame(int gameId)
		{
			await TriggerUpdate();
			Cache.TryGetValue(gameId, out var gameItem);
			return gameItem;
		}

		public async Task<IEnumerable<GameCacheItem>> GetGames()
		{
			await TriggerUpdate();
			return Cache.Values;
		}

		protected override string CacheName
		{
			get { return nameof(GameCache); }
		}

		protected override ConnectionMultiplexer Connection
		{
			get { return RedisConnectionFactory.GetCacheConnection(); }
		}

		protected override TimeSpan ExpireTime
		{
			get { return _expireTimeConfig; }
		}

		protected override async Task<IEnumerable<GameCacheItem>> QueryInitial()
		{
			using (var connction = new SqlConnection(ConnectionString.DefaultConnection))
			{
				return await connction.QueryAsync<GameCacheItem>(StoredProcedure.Cache_Game_GetInitial, commandType: CommandType.StoredProcedure);
			}
		}

		protected override async Task<IEnumerable<GameCacheItem>> QueryUpdates(byte[] lastVersion)
		{
			using (var connction = new SqlConnection(ConnectionString.DefaultConnection))
			{
				return await connction.QueryAsync<GameCacheItem>(StoredProcedure.Cache_Game_GetUpdates, new { LastUpdate = lastVersion }, commandType: CommandType.StoredProcedure);
			}
		}
	}
}
