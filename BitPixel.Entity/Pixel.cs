﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class Pixel
	{
		[Key]
		public int Id { get; set; }
		public int GameId { get; set; }
		public int UserId { get; set; }
		public int? TeamId { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int Points { get; set; }
		public DateTime LastUpdate { get; set; }
		public byte[] LastUpdated { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("TeamId")]
		public virtual Team Team { get; set; }

		public virtual ICollection<PixelHistory> History { get; set; }
	}
}
