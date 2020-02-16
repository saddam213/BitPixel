
using System.ComponentModel.DataAnnotations;
using DotMatrix.Enums;

namespace DotMatrix.Common.Game
{
	public class UpdateGameModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; }

		[Required]
		[MaxLength(256)]
		public string Description { get; set; }
		public GameStatus Status { get; set; }
		public int Rank { get; set; }
		public int ClicksPerSecond { get; set; }
		public GamePlatform Platform { get; set; }
	}
}
