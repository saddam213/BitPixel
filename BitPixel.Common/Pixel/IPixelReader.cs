using System.Collections.Generic;
using System.Threading.Tasks;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;

namespace BitPixel.Common.Pixel
{
	public interface IPixelReader
	{
		Task<List<PixelModel>> GetPixels(int gameId);
		Task<PixelModel> GetPixel(int gameId, int x, int y);

		Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? maxCount);
	}
}
