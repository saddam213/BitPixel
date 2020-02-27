using DotMatrix.Enums;

namespace DotMatrix.Cache.Common
{
	public class PixelCacheItem : IDynamicTableCacheItem<string>
	{
		public string Id { get; set; }
		public byte[] LastUpdated { get; set; }

		public int GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public PixelType Type { get; set; }
		public string Color { get; set; }
		public int Points { get; set; }
	}
}
