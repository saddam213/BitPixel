using System.Collections.Generic;
using System.Threading.Tasks;
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;
using DotMatrix.Entity;

namespace DotMatrix.Common.Users
{
	public interface IUserReader
	{
		Task<UserModel> GetUser(int id);
		Task<List<UserModel>> GetUsers();

		Task<UserProfileModel> GetUserProfile(string userName);
		Task<DataTablesResponseData> GetUsers(DataTablesParam model);
	}
}
