using Cryptopia.QueueService.Implementation;

namespace Cryptopia.QueueService.DataObjects
{
	public class SubmitPixelsResponse : IQueueResponse
	{
		public string Message { get; set; }

		public bool Success { get; set; }
	}
}
