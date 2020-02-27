using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitPixel.Entity
{
	public class AwardHistory
	{
		[Key]
		public int Id { get; set; }
		public int AwardId { get; set; }
		public int UserId { get; set; }
		public int? GameId { get; set; }
		public string Version { get; set; }
		public string VersionData { get; set; }
		public string Data { get; set; }
		public int Points { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("AwardId")]
		public virtual Award Award { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
