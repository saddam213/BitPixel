using BitPixel.Enums;

namespace BitPixel.Common.Pixel
{
	public class AddClickRequest
	{
		public AddClickRequest() { }
		public AddClickRequest(int gameId, int x, int y)
		{
			X = x;
			Y = y;
			GameId = gameId;
		}

		public int GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
}
