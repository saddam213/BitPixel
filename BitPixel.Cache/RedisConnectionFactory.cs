using System;
using System.Configuration;

using StackExchange.Redis;

namespace BitPixel.Cache
{
	public class RedisConnectionFactory
	{
		private static readonly Lazy<ConnectionMultiplexer> CacheConnection;

		static RedisConnectionFactory()
		{
			var cacheOptions = GetOptions(ConfigurationManager.AppSettings["RedisConnection_Cache"]);
			CacheConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(cacheOptions));
		}

		public static ConnectionMultiplexer GetCacheConnection()
		{
			return CacheConnection.Value;
		}

		private static ConfigurationOptions GetOptions(string connectionString)
		{
			var options = ConfigurationOptions.Parse(connectionString);
			options.AbortOnConnectFail = false;
			options.KeepAlive = 10;
			options.ConnectTimeout = 5000;
			options.SyncTimeout = 5000;
			options.ConnectRetry = 10;
			options.AllowAdmin = true;
			return options;
		}
	}
}
