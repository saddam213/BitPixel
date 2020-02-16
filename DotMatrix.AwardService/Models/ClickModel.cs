using System;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class ClickModel
	{
		public long Id { get; set; }
		public int UserId { get; set; }
		public int GameId { get; set; }
		public ClickType ClickType { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
