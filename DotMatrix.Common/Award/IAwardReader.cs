using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotMatrix.Common.Award
{
	public interface IAwardReader
	{
		Task<List<AwardItemModel>> GetAwards();
		Task<List<AwardHistoryItemModel>> GetAwardHistory();

		Task<List<AwardUserHistoryItemModel>> GetUserAwards(int userId);
		Task<AwardUserHistoryItemModel> GetUserAward(int userId, int awardId);
	}
}
