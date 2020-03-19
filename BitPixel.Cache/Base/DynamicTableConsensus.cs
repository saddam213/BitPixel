using System;
using BitPixel.Base.Extensions;

namespace BitPixel.Cache
{
	public class DynamicTableConsensus
	{
		public DynamicTableConsensus() { }
		public DynamicTableConsensus(byte[] version, TimeSpan expiry)
		{
			LastUpdated = version;
			Timestamp = DateTime.UtcNow.RoundToNearest(expiry).Add(expiry).ToUnixMs();
		}

		public byte[] LastUpdated { get; set; }
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
