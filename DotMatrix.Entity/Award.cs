using System;
using System.ComponentModel.DataAnnotations;

using DotMatrix.Enums;

namespace DotMatrix.Entity
{
	public class Award
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public AwardType Type { get; set; }
		public AwardTriggerType TriggerType { get; set; }
		public AwardStatus Status { get; set; }
		public int Points { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
