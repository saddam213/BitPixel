using DotMatrix.Enums;

namespace DotMatrix.Common.Pixel
{
	public class PixelModel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int Points { get; set; }
		public string Player { get; set; }
		public string Team { get; set; }
	}
}
