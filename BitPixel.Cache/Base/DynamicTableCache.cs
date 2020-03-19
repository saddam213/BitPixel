using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BitPixel.Base;
using BitPixel.Base.Extensions;
using BitPixel.Base.Objects;
using BitPixel.Cache.Common;

using StackExchange.Redis;

namespace BitPixel.Cache
{
	/// <summary>
	///   Cache implemetation for caching database data in memory dynamicly,
	///   This instance will track any row changes and update the corresponding cache items
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class DynamicTableCache<T, U> : RedisCache
			where T : ICacheItem<U>, IDynamicTableCacheItem<U>
	{
		private readonly string _nodeKey;
		private readonly ConcurrentLock _queryLock;

		public DynamicTableCache(string nodeKey)
		{
			_nodeKey = nodeKey;
			_queryLock = new ConcurrentLock();
		}

		public virtual async Task<IEnumerable<T>> GetItems()
		{
			await TriggerUpdate();
			if (Cache.IsNullOrEmpty())
				return Enumerable.Empty<T>();

			return Cache.Values;
		}

		protected abstract string CacheName { get; }
		protected abstract TimeSpan ExpireTime { get; }
		protected abstract Task<IEnumerable<T>> QueryInitial();
		protected abstract Task<IEnumerable<T>> QueryUpdates(byte[] lastVersion);
		protected ConcurrentDictionary<U, T> Cache { get; } = new ConcurrentDictionary<U, T>();
		protected ConcurrentValue<DynamicTableConsensus> Consensus { get; } = new ConcurrentValue<DynamicTableConsensus>();

		protected string NodeName
		{
			get { return $"{Environment.MachineName}-{_nodeKey}"; }
		}

		public virtual async Task<byte[]> Initialize()
		{
			if (_queryLock.Lock())
			{
				try
				{
					var initialResults = await QueryInitial();
					var headVersion = GetHead(new byte[8], initialResults);
					await AddOrUpdateItems(initialResults);

					var consensus = new DynamicTableConsensus(headVersion, ExpireTime);
					Consensus.Set(consensus);
					return headVersion;
				}
				finally
				{
					_queryLock.Release();
				}
			}

			// Another thread has a lock, wait for it to complete
			while (_queryLock.IsLocked())
				await Task.Delay(50);

			Consensus.Get(out var currentVersion);
			return currentVersion.LastUpdated;
		}

		protected virtual Task AddOrUpdateItems(IEnumerable<T> newValues)
		{
			if (newValues.IsNullOrEmpty())
				return Task.FromResult(0);

			foreach (var newValue in newValues)
				Cache.AddOrUpdate(newValue.Id, newValue, newValue.GetUpdatedVersion<U, T>());

			return Task.FromResult(0);
		}

		protected async Task TriggerUpdate()
		{
			if (Consensus.Get(out var consensus) && DateTime.UtcNow.ToUnixMs() > consensus.Timestamp)
			{
				if (_queryLock.Lock())
				{
					try
					{
						var newValues = await QueryUpdates(consensus.LastUpdated);
						var newVersion = GetHead(consensus.LastUpdated, newValues);
						await AddOrUpdateItems(newValues);

						var newConsensus = new DynamicTableConsensus(newVersion, ExpireTime);
						SetConsensus(consensus, newConsensus);
					}
					finally
					{
						_queryLock.Release();
					}
				}
			}
		}

		protected byte[] GetHead(byte[] previousHead, IEnumerable<T> items)
		{
			if (items.IsNullOrEmpty())
				return previousHead;

			var currentHead = items
				.OrderByDescending(x => x.LastUpdated.ToUInt64())
				.FirstOrDefault();
			if (currentHead == null)
				return previousHead;

			if (previousHead.IsGreaterThan(currentHead.LastUpdated))
				return previousHead;

			return currentHead.LastUpdated;
		}

		private void SetConsensus(DynamicTableConsensus consensus, DynamicTableConsensus newConsensus)
		{
			Consensus.Update(newConsensus, consensus);
		}


	}
}
