using BitPixel.Enums;

namespace BitPixel.Common.Game
{

	public class GamePrizeModel
	{
		public int Id { get; set; }
		public int GameId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Points { get; set; }
		public PrizeType Type { get; set; }
		public int Rank { get; set; }
		public string Symbol { get; set; }
	}
}
