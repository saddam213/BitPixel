using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class AwardModalViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public int Points { get; set; }
		public AwardTriggerType TriggerType { get; set; }
		public AwardLevel Level { get; set; }
		public string Description { get; set; }
		public ClickType ClickType { get; set; }
	}
}
