﻿using System;

using BitPixel.Enums;

namespace BitPixel.Common.Game
{
	public class GameModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public GameType Type { get; set; }
		public GameStatus Status { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public DateTime Timestamp { get; set; }
		public string Description { get; set; }
		public int Rank { get; set; }
		public int ClicksPerSecond { get; set; }
		public GamePlatform Platform { get; set; }
		public GameEndType EndType { get; set; }
		public DateTime? EndTime { get; set; }
	}
}
