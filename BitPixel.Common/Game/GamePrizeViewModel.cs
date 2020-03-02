
using System;
using System.Collections.Generic;
using BitPixel.Common.Prize;
using BitPixel.Enums;

namespace BitPixel.Common.Game
{
	public class GamePrizeViewModel
	{
		public List<GamePrizeModel> GamePrizes { get; set; }
		public List<PrizeItemModel> PixelPrizes { get; set; }
		public int GameId { get; set; }
		public int Rank { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public GameEndType EndType { get; set; }
		public DateTime? EndTime { get; set; }
		public GameStatus Status { get; set; }
	}
}
