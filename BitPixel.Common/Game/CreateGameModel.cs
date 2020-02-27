
using System.ComponentModel.DataAnnotations;
using BitPixel.Enums;

namespace BitPixel.Common.Game
{
	public class CreateGameModel
	{
		[Required]
		[MaxLength(50)]
		public string Name { get; set; }

		[Required]
		[MaxLength(256)]
		public string Description { get; set; }
		public GameType Type { get; set; }

		[Range(10, 800)]
		public int Width { get; set; }

		[Range(10, 600)]
		public int Height { get; set; }
		public int Rank { get; set; }
		public GamePlatform Platform { get; set; }
		public int ClicksPerSecond { get; set; }
	}
}
