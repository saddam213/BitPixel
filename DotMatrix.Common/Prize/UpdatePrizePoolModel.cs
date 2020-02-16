using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotMatrix.Common.Prize
{
	public class UpdatePrizePoolModel
	{
		public int GameId { get; set; }

		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(128)]
		public string Description { get; set; }

		[MaxLength(50)]
		public string NewName { get; set; }
	}
}
