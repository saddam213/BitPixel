
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
	}

	public class PixelHubAddPrizeRequest
	{
		public int UserId { get; set; }
		public int PrizeId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string PrizeName { get; set; }
		public int PrizePoints { get; set; }
		public int UserPoints { get; set; }
		public string PrizeUser { get; internal set; }
		public string PrizeDescription { get; internal set; }
	}

	public class PixelHubPrizeUpdate
	{
		public int PrizeId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string PrizeName { get; set; }
		public int PrizePoints { get; set; }
		public string PrizeUser { get; internal set; }
	}

	public class PixelHubNewPrizeUpdate
	{
		public int PrizeId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string PrizeName { get; set; }
		public int PrizePoints { get; set; }
		public string PrizeDescription { get; internal set; }
	}
}