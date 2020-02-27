using System.Collections.Generic;
using BitPixel.Common.Award;
using BitPixel.Common.Prize;

namespace BitPixel.Common.Points
{
	public class PointsModel
	{
		public int Points { get; set; }
		public List<PrizeUserHistoryItemModel> LatestPrizes { get; set; } = new List<PrizeUserHistoryItemModel>();

		public List<AwardUserListItemModel> AwardList { get; set; } = new List<AwardUserListItemModel>();
	}
}
