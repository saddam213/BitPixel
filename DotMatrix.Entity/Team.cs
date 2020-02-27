using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotMatrix.Entity
{
	public class Team
	{
		[Key]
		public int Id { get; set; }
		public int GameId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public string Color { get; set; }
		public int Rank { get; set; }
		public int Result { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }
		public virtual ICollection<TeamMember> Members { get; set; }
		public virtual ICollection<Pixel> Pixels { get; set; }
		public virtual ICollection<PixelHistory> PixelHistory { get; set; }
	}
}
