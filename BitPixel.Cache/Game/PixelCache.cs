using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using BitPixel.Cache.Common;
using BitPixel.Common.DataContext;

using StackExchange.Redis;

namespace BitPixel.Cache
{
	public class PixelCache : DynamicTableCache<PixelCacheItem, string>, IPixelCache
	{
		private readonly TimeSpan _expireTimeConfig = TimeSpan.FromMilliseconds(500);

		public static async Task<IPixelCache> BuildCache(string nodeKey)
		{
			var cache = new PixelCache(nodeKey);
			await cache.Initialize();
			return cache;
		}

		public PixelCache(string nodeKey)
			: base(nodeKey) { }
	
		public async Task<PixelCacheItem> GetPixel(int gameId, int x, int y)
		{
			await TriggerUpdate();
			Cache.TryGetValue($"{gameId}-{x}-{y}", out var gameItem);
			return gameItem;
		}

		public async Task<IEnumerable<PixelCacheItem>> GetPixels(int gameId)
		{
			await TriggerUpdate();
			return Cache.Values.Where(x => x.GameId == gameId);
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

		protected override async Task<IEnumerable<PixelCacheItem>> QueryInitial()
		{
			using (var connction = new SqlConnection(ConnectionString.DefaultConnection))
			{
				return await connction.QueryAsync<PixelCacheItem>(StoredProcedure.Cache_Pixel_GetInitial, commandType: CommandType.StoredProcedure);
			}
		}

		protected override async Task<IEnumerable<PixelCacheItem>> QueryUpdates(byte[] lastVersion)
		{
			using (var connction = new SqlConnection(ConnectionString.DefaultConnection))
			{
				return await connction.QueryAsync<PixelCacheItem>(StoredProcedure.Cache_Pixel_GetUpdates, new { LastUpdate = lastVersion }, commandType: CommandType.StoredProcedure);
			}
		}
	}
}
