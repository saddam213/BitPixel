
using DotMatrix.Enums;

namespace DotMatrix
{
	public class PixelHubPixelUpdate
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int NewPoints { get; set; }

		public string Owner { get; set; }
		public string Team { get; set; }
		public bool IsApi { get; set; }
	}
}