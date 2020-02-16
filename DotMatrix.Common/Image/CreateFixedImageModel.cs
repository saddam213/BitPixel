using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Image
{
	public class CreateFixedImageModel
	{
		public int GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public bool IsFixed { get; set; }
		public Stream ImageStream { get; set; }
	}
}
