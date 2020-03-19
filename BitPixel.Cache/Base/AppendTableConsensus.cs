using System;
using BitPixel.Base.Extensions;

namespace BitPixel.Cache
{
	public class AppendTableConsensus<T>
	{
		public AppendTableConsensus() { }
		public AppendTableConsensus(T tail, T head, TimeSpan expiry)
		{
			Tail = tail;
			Head = head;
			Timestamp = DateTime.UtcNow.RoundToNearest(expiry).Add(expiry).ToUnixMs();
		}

		public T Head { get; set; }
		public T Tail { get; set; }
		public long Timestamp { get; set; }

		public override bool Equals(object obj)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
