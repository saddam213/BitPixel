using System;
using StackExchange.Redis;

namespace BitPixel.Cache
{
	public abstract class RedisCache
	{
		protected abstract ConnectionMultiplexer Connection { get; }

		protected virtual IDatabase Database
		{
			get { return Connection.GetDatabase(); }
		}

		protected virtual ISubscriber Subscriber
		{
			get { return Connection.GetSubscriber(); }
		}

		protected T SafeConvert<T>(RedisValue value) where T : IConvertible
		{
			return value.SafeConvert<T>();
		}
	}
}
