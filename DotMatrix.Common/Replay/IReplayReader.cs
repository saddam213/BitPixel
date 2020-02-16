using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Common.Pixel;

namespace DotMatrix.Common.Replay
{
	public interface IReplayReader
	{
		Task<List<ReplayPixelModel>> GetPixels(ReplayFilterModel model);
	}
}
