using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotMatrix.Enums;

namespace DotMatrix.Entity
{
	public class Pixel
	{
		[Key]
		public int Id { get; set; }
		public int UserId { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int Points { get; set; }
		public DateTime LastUpdate { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
		public virtual ICollection<PixelHistory> History { get; set; }
	}
}
