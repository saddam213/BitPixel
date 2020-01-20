namespace DotMatrix.Common.Pixel
{

	public class AddClickResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }

		public bool IsPrizeWinner { get; set; }
		public int PrizeId { get; set; }
		public string PrizeName { get; set; }
		public int PrizePoints { get; set; }
		public int UserPoints { get; set; }
		public string PrizeDescription { get; set; }
	}
}
