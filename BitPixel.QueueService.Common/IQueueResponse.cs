namespace BitPixel.QueueService.Common
{
	public interface IQueueResponse
	{
		bool Success { get; set; }
		string Message { get; set; }
	}
}
