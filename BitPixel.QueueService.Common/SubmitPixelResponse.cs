namespace BitPixel.QueueService.Common
{
	public class SubmitPixelResponse : IQueueResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
	
		public PixelNotification PixelNotification { get; set; }
		public PrizeNotification PrizeNotification { get; set; }
		public PointsNotification PointsNotification { get; set; }
	}
}
