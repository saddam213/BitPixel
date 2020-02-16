using System.Collections.Generic;
using DotMatrix.Common.Award;
using DotMatrix.Common.Prize;

namespace DotMatrix.Common.Points
{
	public class PointsModel
	{
		public int Points { get; set; }
		public List<PrizeUserHistoryItemModel> LatestPrizes { get; set; } = new List<PrizeUserHistoryItemModel>();

		public List<AwardUserListItemModel> AwardList { get; set; } = new List<AwardUserListItemModel>();
	}
}
