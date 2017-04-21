using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Pixel
{
	public interface IPixelWriter
	{
		Task<bool> AddOrUpdate(string userId, PixelModel model);
	}
}
