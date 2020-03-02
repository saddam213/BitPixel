using System;
using System.Collections.Generic;
using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class Game
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public GameType Type { get; set; }
		public GameStatus Status { get; set; }
		public GamePlatform Platform { get; set; }
		public GameEndType EndType { get; set; }
		public DateTime? EndTime { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int ClicksPerSecond { get; set; }
		public int Rank { get; set; }
		public DateTime Timestamp { get; set; }
		public byte[] LastUpdated { get; set; }

		public virtual ICollection<Team> Teams { get; set; }
		public virtual ICollection<Pixel> Pixels { get; set; }
		public virtual ICollection<PixelHistory> PixelHistory { get; set; }
		public virtual ICollection<Prize> Prizes { get; set; }
		public virtual ICollection<GamePrize> GamePrizes { get; set; }
	}
}
