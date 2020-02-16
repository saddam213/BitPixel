using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class AwardListItemModel
	{
		public int AwardId { get; set; }

		public string Icon { get; set; }
		public string Name { get; set; }
		public AwardLevel Level { get; set; }
		public ClickType ClickType { get; set; }
		public int Points { get; set; }
		public int Rank { get; set; }
	}
}
