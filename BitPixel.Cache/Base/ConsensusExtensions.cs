using System;
using System.Linq;

using BitPixel.Base;
using BitPixel.Cache.Common;

using StackExchange.Redis;

namespace BitPixel.Cache
{
	public static class ConsensusExtensions
	{
		public static RedisValue ToCacheEntry(this DynamicTableConsensus consensus)
		{
			return $"{consensus.LastUpdated.ToUInt64()},{consensus.Timestamp}";
		}

		public static DynamicTableConsensus ToConsensus(this HashEntry cacheEntry)
		{
			if (string.IsNullOrEmpty(cacheEntry.Name) || string.IsNullOrEmpty(cacheEntry.Value))
				return default(DynamicTableConsensus);

			var consensusData = cacheEntry.Value.ToString().Split(',').ToList();
			if (consensusData.Count < 2)
				return default(DynamicTableConsensus);

			return new DynamicTableConsensus
			{
				LastUpdated = SafeConvert<ulong>(consensusData[0]).ToByteArray(),
				Timestamp = SafeConvert<long>(consensusData[1])
			};
		}

		public static RedisValue ToCacheEntry<V>(this AppendTableConsensus<V> consensus)
		{
			return $"{consensus.Tail},{consensus.Head},{consensus.Timestamp}";
		}

		public static AppendTableConsensus<T> ToConsensus<T>(this HashEntry cacheEntry) where T : IConvertible
		{
			if (string.IsNullOrEmpty(cacheEntry.Name) || string.IsNullOrEmpty(cacheEntry.Value))
				return default(AppendTableConsensus<T>);

			var consensusData = cacheEntry.Value.ToString().Split(',').ToList();
			if (consensusData.Count < 3)
				return default(AppendTableConsensus<T>);

			return new AppendTableConsensus<T>
			{
				Tail = SafeConvert<T>(consensusData[0]),
				Head = SafeConvert<T>(consensusData[1]),
				Timestamp = SafeConvert<long>(consensusData[2])
			};
		}

		public static T SafeConvert<T>(this RedisValue value) where T : IConvertible
		{
			try
			{ return (T)Convert.ChangeType(value, typeof(T)); }
			catch { return default(T); }
		}

		public static Func<U, T, T> GetUpdatedVersion<U, T>(this T item) where T : IDynamicTableCacheItem<U>
		{
			return (id, existing) => item.LastUpdated.IsGreaterThan(existing.LastUpdated) ? item : existing;
		}
	}
}
