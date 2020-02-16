using System;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class PixelModel
	{
		public long Id { get; set; }
		public int UserId { get; set; }
		public int GameId { get; set; }
		public string Color { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public DateTime Timestamp { get; set; }
		public int Points { get; internal set; }
		public PixelType Type { get; internal set; }
	}
}
