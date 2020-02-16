using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotMatrix.Common.Game;
using DotMatrix.Enums;

namespace DotMatrix.Common.Prize
{
	public class CreatePrizePoolModel
	{
		[Range(1, 50000)]
		public int Count { get; set; }

		[Range(0, 50000)]
		public int MaxPoints { get; set; }

		public PrizeType Type { get; set; }

		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(128)]
		public string Description { get; set; }

		[Range(1, int.MaxValue)]
		public int Points { get; set; }

		public string Data { get; set; }
		public string Data2 { get; set; }

		[Required]
		public int? GameId { get; set; }
		public List<GameModel> Games { get; set; }

	}
}
