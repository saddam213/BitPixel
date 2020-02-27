using System.Threading.Tasks;
using BitPixel.Enums;

namespace BitPixel.Common.Pixel
{
	public interface IPixelWriter
	{
		Task<AddClickResponse> AddClick(int userId, AddClickRequest request);
		Task<AddPixelResponse> AddPixel(int userId, AddPixelRequest request);
	}
}
