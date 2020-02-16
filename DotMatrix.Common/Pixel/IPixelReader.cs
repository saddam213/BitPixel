using System.Collections.Generic;
using System.Threading.Tasks;
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;

namespace DotMatrix.Common.Pixel
{
	public interface IPixelReader
	{
		Task<List<PixelModel>> GetPixels(int gameId);
		Task<PixelModel> GetPixel(int gameId, int x, int y);

		Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? maxCount);
	}
}
