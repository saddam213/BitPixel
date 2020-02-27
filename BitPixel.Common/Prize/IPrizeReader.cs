using System.Collections.Generic;
using System.Threading.Tasks;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;

namespace BitPixel.Common.Prize
{
	public interface IPrizeReader
	{
		Task<PrizeHistoryItemModel> GetPrize(int prizeId);
		Task<List<PrizeItemModel>> GetPrizes();
		Task<List<PrizeItemModel>> GetPrizes(int gameId);

		Task<PrizeUserHistoryItemModel> GetUserPrize(int userId, int prizeId);



		Task<PrizeUserHistoryItemModel> GetPrizePayment(int prizeId);
		Task<List<PrizeUserHistoryItemModel>> GetPrizePayments();

		Task<DataTablesResponseData> GetHistory(DataTablesParam model);
		Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? count);
		Task<DataTablesResponseData> GetGameHistory(DataTablesParam model, int gameId);
		Task<DataTablesResponseData> GetPrizePayments(DataTablesParam model);
	}
}
