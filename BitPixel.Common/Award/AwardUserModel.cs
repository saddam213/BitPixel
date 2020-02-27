using BitPixel.Enums;

namespace BitPixel.Common.Award
{
	public class AwardUserModel
	{
		public int Id { get; set; }
		public string Icon { get; set; }
		public string Name { get; set; }
		public int Points { get; set; }
		public AwardTriggerType TriggerType { get; set; }
		public string Description { get; set; }
		public AwardLevel Level { get; set; }
		public int Count { get; set; }
		public ClickType ClickType { get; set; }
	}
}
