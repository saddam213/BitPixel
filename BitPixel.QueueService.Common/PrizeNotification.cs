namespace BitPixel.QueueService.Common
{
	public class PrizeNotification
	{
		public int PrizeId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Points { get; set; }

		public int UserId { get; set; }
		public string UserName { get; set; }

		public int GameId { get; set; }
		public string GameName { get; set; }
	}
}
