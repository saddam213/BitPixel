using System.Threading.Tasks;
using DotMatrix.Enums;

namespace DotMatrix.Common.Pixel
{
	public interface IPixelWriter
	{
		Task<AddClickResponse> AddClick(int userId, AddClickRequest request);
		Task<AddPixelResponse> AddPixel(int userId, AddPixelRequest request);
	}
}
