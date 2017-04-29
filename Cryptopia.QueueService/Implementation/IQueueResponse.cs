namespace Cryptopia.QueueService.Implementation
{
	public interface IQueueResponse
	{
		bool Success { get; set; }
		string Message { get; set; }
	}
}
