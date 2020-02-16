using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Replay
{
	public class ReplayViewModel
	{
		public int GameId { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public List<string> Players { get; set; }
	}

}
