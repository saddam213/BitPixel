
using DotMatrix.Enums;

namespace DotMatrix
{
	public class PixelHubAddPixelRequest
	{
		public int PixelId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }

		public int UserId { get; set; }
		public string UserName { get; set; }
		public int UserPoints { get; set; }
		public int TeamId { get; set; }
		public string TeamName { get; set; }
		public int NewPoints { get; set; }

		public bool IsApi { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; }
	}
}