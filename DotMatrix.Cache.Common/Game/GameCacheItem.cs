using System;
using DotMatrix.Cache.Common;
using DotMatrix.Enums;

namespace DotMatrix.Cache.Common
{
	public class GameCacheItem : IDynamicTableCacheItem<int>
	{
		public int Id { get; set; }
		public byte[] LastUpdated { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }
		public GameType Type { get; set; }
		public GameStatus Status { get; set; }
		public GamePlatform Platform { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int ClicksPerSecond { get; set; }
		public int Rank { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
