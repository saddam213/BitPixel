namespace BitPixel.QueueService.Common
{
	public class QueueErrorResponse : IQueueResponse
	{
		public QueueErrorResponse() { }
		public QueueErrorResponse(string error)
		{
			Success = false;
			Message = error;
		}
		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
