
using DotMatrix.Enums;

namespace DotMatrix
{
	public class PixelHubAwardRequest
	{
		public int UserId { get; set; }
		public int AwardId { get; set; }
		public string AwardName { get; set; }
		public AwardLevel AwardLevel { get; set; }
		public int AwardPoints { get; set; }
		public int UserPoints { get; set; }
	}
}