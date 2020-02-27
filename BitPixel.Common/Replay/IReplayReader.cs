using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Common.Pixel;

namespace BitPixel.Common.Replay
{
	public interface IReplayReader
	{
		Task<List<ReplayPixelModel>> GetPixels(ReplayFilterModel model);
	}
}
