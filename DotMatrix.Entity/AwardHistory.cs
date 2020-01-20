using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotMatrix.Entity
{
	public class AwardHistory
	{
		[Key]
		public int Id { get; set; }
		public int AwardId { get; set; }
		public int UserId { get; set; }
		public string Version { get; set; }
		public string VersionData { get; set; }
		public int Points { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("AwardId")]
		public virtual Award Award { get; set; }
	}
}
