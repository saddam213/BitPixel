using System.Collections.Generic;

namespace DotMatrix.Common.Score
{
	public class ScoreViewModel
	{
		public List<ScoreViewItemModel> PixelBoard { get; set; } = new List<ScoreViewItemModel>();
		public List<ScoreViewItemModel> PrizeBoard { get; set; } = new List<ScoreViewItemModel>();
		public List<ScoreViewItemModel> AwardBoard { get; set; } = new List<ScoreViewItemModel>();
		public List<ScoreViewItemModel> ClickBoard { get; set; } = new List<ScoreViewItemModel>();
		public List<ScoreViewItemModel> PointsWonBoard { get; set; } = new List<ScoreViewItemModel>();
		public List<ScoreViewItemModel> PointsLostBoard { get; set; } = new List<ScoreViewItemModel>();
	}
}
