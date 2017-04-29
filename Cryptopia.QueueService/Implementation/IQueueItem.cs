namespace Cryptopia.QueueService.Implementation
{
	public interface IQueueItem
	{
		string UserId { get; set; }
		bool IsApi { get; set; }
	}
}
