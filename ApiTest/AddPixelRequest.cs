using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest
{
	public class AddPixelRequest
	{
		public int X { get; set; }
		public int Y { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
	}
}
