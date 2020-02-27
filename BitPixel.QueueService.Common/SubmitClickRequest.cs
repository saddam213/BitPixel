namespace BitPixel.QueueService.Common
{
	public class SubmitClickRequest : IQueueRequest
	{
		public int UserId { get; set; }
		public int GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
}
