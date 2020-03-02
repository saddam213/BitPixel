﻿using System.Collections.Generic;
using BitPixel.Common.Game;

namespace BitPixel.Common.Prize
{
	public class ViewPrizeModalModel
	{
		public GameModel Game { get; set; }
		public PrizeHistoryItemModel Prize { get; set; }
	}

	public class ViewUserPrizeModalModel
	{
		public GameModel Game { get; set; }
		public PrizeUserHistoryItemModel Prize { get; set; }
	}

	public class ViewPrizesModalModel
	{
		public GamePrizeViewModel Prizes { get; set; }
	}

	public class PrizesViewModel
	{
		public List<GamePrizeViewModel> Prizes { get; set; }
	}
}
