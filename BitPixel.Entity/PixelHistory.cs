﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class PixelHistory
	{
		[Key]
		public int Id { get; set; }
		public int PixelId { get; set; }
		public int GameId { get; set; }
		public int UserId { get; set; }
		public int? TeamId { get; set; }

		public string Color { get; set; }
		public PixelType Type { get; set; }
		public int Points { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("PixelId")]
		public virtual Pixel Pixel { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("TeamId")]
		public virtual Team Team { get; set; }
	}
}
