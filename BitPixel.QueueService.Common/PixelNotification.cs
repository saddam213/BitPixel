using BitPixel.Enums;

namespace BitPixel.QueueService.Common
{
	public class PixelNotification
	{
		public int PixelId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int Points { get; set; }

		public int UserId { get; set; }
		public string UserName { get; set; }

		public int GameId { get; set; }
		public string GameName { get; set; }
	}
}
