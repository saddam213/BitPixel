namespace BitPixel.QueueService.Common
{
	public class SubmitClickResponse : IQueueResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
