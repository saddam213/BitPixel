using System.Collections.Generic;
using BitPixel.Common.Game;
using BitPixel.Common.Team;
using BitPixel.Enums;

namespace BitPixel.Common.Pixel
{
	public class PixelViewlModel
	{
		public int Points { get; set; }
		public GameModel Game { get; set; }
		public List<TeamModel> Teams { get; set; }
		public TeamModel Team { get; set; }
	}
}
