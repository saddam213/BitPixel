using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Pixel
{
	public class PixelResultModel
	{
		public string UserId { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		public decimal Balance { get; set; }
	}
}
