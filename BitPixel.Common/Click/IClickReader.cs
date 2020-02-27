using System.Threading.Tasks;

using BitPixel.Datatables;
using BitPixel.Datatables.Models;

namespace BitPixel.Common.Click
{
	public interface IClickReader
	{
		Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? maxCount);
	}
}
