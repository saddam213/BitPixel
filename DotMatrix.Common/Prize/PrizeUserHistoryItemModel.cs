using System;
using DotMatrix.Enums;

namespace DotMatrix.Common.Prize
{
	public class PrizeUserHistoryItemModel
	{
		public int Id { get; set; }

		public PrizeType Type { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public int Points { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }

		public DateTime Timestamp { get; set; }
	}
}
