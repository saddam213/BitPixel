using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class UpdateAwardModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public string Description { get; set; }
		public AwardLevel Level { get; set; }
		public AwardStatus Status { get; set; }
		public int Points { get; set; }
		public int Rank { get; set; }
	}
}
