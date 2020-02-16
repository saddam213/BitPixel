using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DotMatrix.Enums;

namespace DotMatrix.Entity
{
	public class Click
	{
		[Key]
		public long Id { get; set; }
		public int GameId { get; set; }
		public int UserId { get; set; }
		public ClickType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
