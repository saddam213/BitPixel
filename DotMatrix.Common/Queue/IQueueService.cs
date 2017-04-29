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
		Task<PixelResultModel> SubmitPixel(string userId, PixelModel model, bool isApi);
		Task<PixelResultModel> SubmitPixels(string userId, List<PixelModel> model, bool isApi);
	}
}
