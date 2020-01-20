using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotMatrix.Common.Prize
{
	public interface IPrizeReader
	{
		Task<List<PrizeItemModel>> GetPrizes();
		Task<List<PrizeHistoryItemModel>> GetPrizeHistory();

		Task<List<PrizeUserHistoryItemModel>> GetUserPrizes(int userId);
		Task<PrizeUserHistoryItemModel> GetUserPrize(int userId, int prizeId);
	}
}
