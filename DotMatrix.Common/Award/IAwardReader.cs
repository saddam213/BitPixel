using System.Collections.Generic;
using System.Threading.Tasks;
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;

namespace DotMatrix.Common.Award
{
	public interface IAwardReader
	{
		Task<List<AwardModel>> GetAwards();
		Task<AwardModel> GetAward(int awardId);

		Task<AwardUserModel> GetUserAward(int userId, int awardId);

		Task<List<AwardListItemModel>> GetAwardList();
		Task<List<AwardUserListItemModel>> GetUserAwardList(int userId);

		Task<DataTablesResponseData> GetAwards(DataTablesParam model);
		Task<DataTablesResponseData> GetHistory(DataTablesParam model);
		Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId);
		Task<DataTablesResponseData> GetGameHistory(DataTablesParam model, int gameId);
		
	}
}
