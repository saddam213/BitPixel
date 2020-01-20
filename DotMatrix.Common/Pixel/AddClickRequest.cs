using DotMatrix.Enums;

namespace DotMatrix.Common.Pixel
{
	public class AddClickRequest
	{
		public AddClickRequest() { }
		public AddClickRequest(PixelClickType type, int x, int y)
		{
			X = x;
			Y = y;
			Type = type;
		}

		public int X { get; set; }
		public int Y { get; set; }
		public PixelClickType Type { get; set; }
	}
}
