using System;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class GameModel
	{
		public int Id { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public GameEndType EndType { get; set; }
		public DateTime? EndTime { get; set; }
		public GameStatus Status { get; set; }
	}
}
