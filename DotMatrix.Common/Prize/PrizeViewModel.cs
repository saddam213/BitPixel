using System.Collections.Generic;
using DotMatrix.Common.Game;

namespace DotMatrix.Common.Prize
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
		public GameModel Game { get; set; }
		public List<PrizeItemModel> Prizes { get; set; }
	}

	public class PrizesViewModel
	{
		public List<PrizeItemModel> Prizes { get; set; }
	}
}
