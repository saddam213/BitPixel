using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Common.Game;

namespace DotMatrix.Common.Gallery
{
	public class GalleryViewModel
	{
		public List<GameModel> Games { get; set; }
	}

	public class GalleryGameViewModel
	{
		public GameModel Game { get; set; }
		public List<string> Players { get; set; }
	}
}
