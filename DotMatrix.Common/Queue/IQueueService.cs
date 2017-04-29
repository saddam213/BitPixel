using DotMatrix.Common.Pixel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Queue
{
	public interface IQueueService
	{
		Task<bool> SubmitPixel(string userId, PixelModel model, bool isApi);
		Task<bool> SubmitPixels(string userId, List<PixelModel> model, bool isApi);
	}
}
