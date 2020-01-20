using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotMatrix.Common.Pixel
{
	public interface IPixelReader
	{
		Task<List<PixelModel>> GetPixels();
		Task<PixelModel> GetPixel(int x, int y);
	}
}
