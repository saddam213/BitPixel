using System;
using System.ComponentModel.DataAnnotations;

using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class Award
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public string Description { get; set; }
		public AwardType? Type { get; set; }
		public AwardLevel Level { get; set; }
		public AwardTriggerType TriggerType { get; set; }
		public ClickType ClickType { get; set; }
		public AwardStatus Status { get; set; }
		public int Points { get; set; }
		public int Rank { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
