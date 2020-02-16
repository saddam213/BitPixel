using System.Threading.Tasks;

using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;

namespace DotMatrix.Common.Click
{
	public interface IClickReader
	{
		Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? maxCount);
	}
}
