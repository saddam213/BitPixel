using System.Collections.Generic;

namespace DotMatrix.Common.Score
{

	public class ScoresModel
	{
		public List<ScoreItemModel> PixelBoard { get; set; } = new List<ScoreItemModel>();
		public List<ScoreItemModel> PrizeBoard { get; set; } = new List<ScoreItemModel>();
		public List<ScoreItemModel> AwardBoard { get; set; } = new List<ScoreItemModel>();
		public List<ScoreItemModel> ClickBoard { get; set; } = new List<ScoreItemModel>();
	}
}
