using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class GamePrize
	{
		[Key]
		public int Id { get; set; }
		public int GameId { get; set; }
		public int? UserId { get; set; }
		public int? TeamId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public PrizeType Type { get; set; }
		public int Points { get; set; }
		public int Rank { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data5 { get; set; }
		public DateTime? ClaimTime { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("UserId")]
		public virtual Team Team { get; set; }
	}
}
