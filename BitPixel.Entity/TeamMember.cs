using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitPixel.Entity
{
	public class TeamMember
	{
		[Key]
		public int Id { get; set; }
		public int TeamId { get; set; }
		public int UserId { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("TeamId")]
		public virtual Team Team { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
