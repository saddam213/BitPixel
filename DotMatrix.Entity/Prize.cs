using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DotMatrix.Enums;

namespace DotMatrix.Entity
{
	public class Prize
	{
		[Key]
		public int Id { get; set; }
		public int GameId { get; set; }
		public PrizeType Type { get; set; }
		public PrizeStatus Status { get; set; }
		public bool IsClaimed { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public int Points { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data5 { get; set; }

		public int? UserId { get; set; }
		public DateTime? ClaimTime { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("GameId")]
		public virtual Game Game { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
