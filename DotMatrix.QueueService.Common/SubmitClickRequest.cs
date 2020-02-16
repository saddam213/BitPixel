using DotMatrix.Enums;

namespace DotMatrix.QueueService.Common
{

	public class SubmitClickRequest : IQueueRequest
	{
		public int UserId { get; set; }

		//public PixelClickType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public bool IsApi { get; set; }
		public int GameId { get; set; }
	}
}
