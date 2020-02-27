using System.Collections.Concurrent;

namespace BitPixel.Base.Objects
{
	public class ConcurrentLock
	{
		private readonly byte _key;
		private readonly ConcurrentDictionary<byte, bool> _lockData;

		public ConcurrentLock()
		{
			_key = 0;
			_lockData = new ConcurrentDictionary<byte, bool>();
		}

		public bool Lock()
		{
			return _lockData.TryAdd(_key, true);
		}

		public bool Release()
		{
			return _lockData.TryRemove(_key, out _);
		}

		public bool IsLocked()
		{
			return _lockData.ContainsKey(_key);
		}
	}


	public class ConcurrentLock<T>
	{
		private readonly ConcurrentDictionary<T, bool> _lockData;

		public ConcurrentLock()
		{
			_lockData = new ConcurrentDictionary<T, bool>();
		}

		public bool Lock(T value)
		{
			return _lockData.TryAdd(value, true);
		}

		public bool Release(T value)
		{
			return _lockData.TryRemove(value, out _);
		}

		public bool IsLocked(T value)
		{
			return _lockData.ContainsKey(value);
		}
	}
}
