using System.Collections.Generic;
using DotMatrix.Common.Game;
using DotMatrix.Common.Team;
using DotMatrix.Enums;

namespace DotMatrix.Common.Pixel
{
	public class PixelViewlModel
	{
		public int Points { get; set; }
		public GameModel Game { get; set; }
		public List<TeamModel> Teams { get; set; }
		public TeamModel Team { get; set; }
	}
}
