using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class AddAwardResult
	{
		public int UserId { get; set; }
		public int AwardId { get; set; }
		public string AwardName { get; set; }
		public int AwardPoints { get; set; }
		public AwardLevel AwardLevel { get; set; }
		public int UserPoints { get; set; }

		public string Error { get; set; }
	}
}
