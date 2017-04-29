using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Pixel
{
	public class PixelModel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public string Color
		{
			get { return $"{R},{G},{B}"; }
		}
		public bool IsValid()
		{
			return true;
		}
	}
}
