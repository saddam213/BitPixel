using System;
using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class AwardModel
	{
		public int Id { get; set; }
		public string Icon { get; set; }
		public string Name { get; set; }
		public int Points { get; set; }
		public DateTime Timestamp { get; set; }
		public AwardTriggerType TriggerType { get; set; }
		public string Description { get; set; }
		public AwardLevel Level { get; set; }
		public ClickType ClickType { get; set; }
		public int Rank { get; set; }
		public AwardStatus Status { get; set; }
	}
}
