using BitPixel.Enums;

namespace BitPixel.Common.Prize
{
	public class PrizeItemModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public PrizeType Type { get; set; }

		public int Count { get; set; }
		public int Unclaimed { get; set; }
		public string Game { get; set; }
		public int GameId { get; set; }
		public int GameRank { get; set; }
		public string Symbol { get; set; }
	}
}
