using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Common.Game;
using BitPixel.Common.Team;

namespace BitPixel.Common.Gallery
{
	public class GalleryViewModel
	{
		public List<GameModel> Games { get; set; }
	}

	public class GalleryGameViewModel
	{
		public GameModel Game { get; set; }
		public List<string> Players { get; set; }
		public TeamModel Team { get; set; }
		public List<TeamModel> Teams { get; set; }
	}
}
