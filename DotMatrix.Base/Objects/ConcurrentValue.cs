using System.Collections.Concurrent;

namespace DotMatrix.Base.Objects
{
	public class ConcurrentValue<T>
	{
		private readonly byte _key;
		private readonly ConcurrentDictionary<byte, T> _lockData;

		public ConcurrentValue()
		{
			_key = 0;
			_lockData = new ConcurrentDictionary<byte, T>();
		}

		public bool IsEmpty
		{
			get { return !_lockData.ContainsKey(_key); }
		}

		public bool Set(T value)
		{
			return _lockData.TryAdd(_key, value);
		}

		public bool Get(out T value)
		{
			return _lockData.TryGetValue(_key, out value);
		}

		public bool Update(T newValue, T comparisonValue)
		{
			return _lockData.TryUpdate(_key, newValue, comparisonValue);
		}
	}
}
