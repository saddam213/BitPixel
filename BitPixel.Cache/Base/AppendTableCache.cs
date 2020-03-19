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
	public abstract class AppendTableCache<T, U> : RedisCache
		where T : ICacheItem<U>
		where U : IComparable, IConvertible
	{
		private readonly string _nodeKey;
		private readonly string _consensusChannel;
		private readonly ConcurrentLock _queryLock;
		private readonly ConcurrentValue<AppendTableConsensus<U>> _consensus;

		public AppendTableCache(string nodeKey)
		{
			_nodeKey = nodeKey;
			_queryLock = new ConcurrentLock();
			_consensus = new ConcurrentValue<AppendTableConsensus<U>>();
			_consensusChannel = $"{CacheName}_Consensus";
			Subscriber.Subscribe(_consensusChannel, OnConsensusChanged);
		}

		public async Task<IEnumerable<T>> GetItems()
		{
			await TriggerUpdate();
			if (Cache.IsEmpty)
				return Enumerable.Empty<T>();

			return Cache.Values;
		}

		protected abstract string CacheName { get; }
		protected abstract TimeSpan ExpireTime { get; }
		protected abstract Task<IEnumerable<T>> QueryInitial();
		protected abstract Task<IEnumerable<T>> QueryUpdates(U lastId);
		protected ConcurrentDictionary<U, T> Cache { get; } = new ConcurrentDictionary<U, T>();

		protected string NodeName
		{
			get { return $"{Environment.MachineName}-{_nodeKey}"; }
		}

		protected virtual async Task Initialize()
		{
			if (_queryLock.Lock())
			{
				try
				{
					var items = await QueryInitial();
					var tailVersion = GetTail(items);
					var headVersion = GetHead(default(U), items);
					await AddItems(items);

					var consensus = new AppendTableConsensus<U>(tailVersion, headVersion, ExpireTime);
					_consensus.Set(consensus);
					await PushConsensus(consensus, true);
				}
				finally
				{
					_queryLock.Release();
				}
			}
		}

		protected virtual Task AddItems(IEnumerable<T> items)
		{
			if (items.IsNullOrEmpty())
				return Task.FromResult(0);

			foreach (var dataItem in items)
				Cache.TryAdd(dataItem.Id, dataItem);

			return Task.FromResult(0);
		}

		protected async Task TriggerUpdate()
		{
			if (_consensus.Get(out var consensus) && DateTime.UtcNow.ToUnixMs() > consensus.Timestamp)
			{
				if (_queryLock.Lock())
				{
					try
					{
						var newItems = await QueryUpdates(consensus.Head);
						var newHead = GetHead(consensus.Head, newItems);
						await AddItems(newItems);

						var newConsensus = new AppendTableConsensus<U>(consensus.Tail, newHead, ExpireTime);
						await SetConsensus(consensus, newConsensus, true);
					}
					finally
					{
						_queryLock.Release();
					}
				}
			}
		}

		private async Task TriggerConsensus()
		{
			if (_queryLock.Lock())
			{
				try
				{
					var newConsensus = await PullConsensus();
					if (newConsensus == default(AppendTableConsensus<U>))
						return;

					if (_consensus.Get(out var currentConsensus))
					{
						// Check if this consensus is newer than the current, if not return here
						if (currentConsensus.Timestamp > newConsensus.Timestamp)
							return;

						// Check Tail is synced
						if (newConsensus.Tail.IsGreaterThan(currentConsensus.Tail))
						{
							var itemsToRemove = Cache.Keys
								.Where(x => x.IsLessThan(newConsensus.Tail))
								.ToList();
							foreach (var item in itemsToRemove)
								Cache.TryRemove(item, out _);
						}

						// Check Head is synced
						if (newConsensus.Head.IsGreaterThan(currentConsensus.Head))
						{
							var itemsToAdd = (await QueryUpdates(currentConsensus.Head))
								.Where(x => x.Id.IsLessOrEqual(newConsensus.Head))
								.ToList();
							await AddItems(itemsToAdd);
						}

						// Set local Consensus
						await SetConsensus(currentConsensus, newConsensus, false);
					}
				}
				finally
				{
					_queryLock.Release();
				}
			}
		}

		private async Task SetConsensus(AppendTableConsensus<U> consensus, AppendTableConsensus<U> newConsensus, bool notifyAllNodes)
		{
			_consensus.Update(newConsensus, consensus);
			await PushConsensus(newConsensus, notifyAllNodes);
		}

		private async Task PushConsensus(AppendTableConsensus<U> consensus, bool notifyAllNodes)
		{
			try
			{
				var cacheEntry = consensus.ToCacheEntry();
				await Database.HashSetAsync(CacheName, NodeName, cacheEntry);
				if (notifyAllNodes)
					await Database.PublishAsync(_consensusChannel, NodeName);
			}
			catch (Exception)
			{
			}
		}

		private async Task<AppendTableConsensus<U>> PullConsensus()
		{
			try
			{
				var cacheData = await Database.HashGetAllAsync(CacheName, CommandFlags.PreferSlave);
				if (cacheData.IsNullOrEmpty())
					return default(AppendTableConsensus<U>);

				var consensusData = cacheData
					.Select(x => x.ToConsensus<U>())
					.ToList();

				// Largest Tail, Head and Timestamp is the consensus
				return consensusData
					.OrderByDescending(x => x.Tail)
					.OrderByDescending(x => x.Head)
					.ThenByDescending(x => x.Timestamp)
					.FirstOrDefault();
			}
			catch (Exception)
			{
				return default(AppendTableConsensus<U>);
			}
		}

		private void OnConsensusChanged(RedisChannel channel, RedisValue value)
		{
			try
			{
				var nodeName = value.ToString();
				if (string.IsNullOrEmpty(nodeName) || nodeName == NodeName)
					return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				TriggerConsensus();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed	
			}
			catch (Exception)
			{
			}
		}

		private U GetHead(U previousHead, IEnumerable<T> items)
		{
			if (items.IsNullOrEmpty())
				return previousHead;

			var currentHead = items.Max(x => x.Id);
			if (previousHead.IsGreaterThan(currentHead))
				return previousHead;

			return currentHead;
		}

		private U GetTail(IEnumerable<T> items)
		{
			if (items.IsNullOrEmpty())
				return default(U);

			return items.Min(x => x.Id);
		}
	}
}
