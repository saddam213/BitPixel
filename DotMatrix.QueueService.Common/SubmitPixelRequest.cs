
using DotMatrix.Enums;

namespace DotMatrix.QueueService.Common
{
	public class SubmitPixelRequest
	{
		public int UserId { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int Points { get; set; }
		public int MaxPoints { get; set; }

		public bool IsApi { get; set; }
	}
}
