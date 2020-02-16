namespace DotMatrix.QueueService.Common
{
	public class SubmitPixelResponse : IQueueResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }

		public int PixelId { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int UserPoints { get; set; }
		public int TeamId { get; set; }
		public string TeamName { get; set; }
		public int NewPoints { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; }
		public bool IsPrizeWinner { get; set; }
		public int PrizeId { get; set; }
		public string PrizeName { get; set; }
		public int PrizePoints { get; set; }
		public string PrizeDescription { get; set; }
	}
}
