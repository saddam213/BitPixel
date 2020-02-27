using System;
using BitPixel.Enums;

namespace BitPixel.Common.Prize
{
	public class PrizeHistoryItemModel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Description { get; set; }
		public int Points { get; set; }
		public PrizeType Type { get; set; }
		public DateTime Timestamp { get; set; }
		public string Game { get; set; }
		public int GameId { get; set; }
	}
}
