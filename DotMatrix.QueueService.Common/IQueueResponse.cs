namespace DotMatrix.QueueService.Common
{
	public interface IQueueResponse
	{
		bool Success { get; set; }
		string Message { get; set; }
	}
}
