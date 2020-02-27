using BitPixel.Enums;

namespace BitPixel.Common.Award
{

	public class AwardUserListItemModel
	{
		public int AwardId { get; set; }
	
		public string Icon { get; set; }
		public string Name { get; set; }
		public AwardLevel Level { get; set; }

		public int Count { get; set; }
		public ClickType ClickType { get; set; }
		public int Rank { get; set; }
	}
}
