namespace DotMatrix
{
	public class PixelHubAddPrizeRequest
	{
		public int UserId { get; set; }
		public int PrizeId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string PrizeName { get; set; }
		public int PrizePoints { get; set; }
		public int UserPoints { get; set; }
		public string PrizeUser { get; set; }
		public string PrizeDescription { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; }
	}
}