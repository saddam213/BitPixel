using System;
using System.Threading.Tasks;
using BitPixel.Cache.Common;

using StackExchange.Redis;

namespace BitPixel.Cache
{
	public class ThrottleCache : RedisCache, IThrottleCache
	{
		public async Task<bool> ShouldExcecute(int userId, string method, TimeSpan period, int maxCalls)
		{
			var throttleKey = $"ThrottleUser:{method}:{userId}";
			var currentCount = await GetExecutionCount(throttleKey, period);
			return currentCount <= maxCalls;
		}

		public async Task<bool> ShouldExcecute(string ipAddress, string method, TimeSpan period, int maxCalls)
		{
			var throttleKey = $"ThrottleIP:{method}:{ipAddress}";
			var currentCount = await GetExecutionCount(throttleKey, period);
			return currentCount <= maxCalls;
		}

		private async Task<long> GetExecutionCount(string throttleKey, TimeSpan period)
		{
			try
			{
				if (!Connection.IsConnected)
					return -1;

			
				var throttleKeyTTL = await Database.KeyTimeToLiveAsync(throttleKey, CommandFlags.PreferSlave);
				if (!throttleKeyTTL.HasValue)
				{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
					var transaction = Database.CreateTransaction();
					transaction.StringIncrementAsync(throttleKey, 1);
					transaction.KeyExpireAsync(throttleKey, period);
					await transaction.ExecuteAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
					return 1;
				}

				return await Database.StringIncrementAsync(throttleKey);
			}
			catch (Exception)
			{
				return -1;
			}
		}


		protected override ConnectionMultiplexer Connection
		{
			get { return RedisConnectionFactory.GetCacheConnection(); }
		}
	}
}
