using System.Collections.Generic;
using System.Threading.Tasks;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;
using BitPixel.Entity;

namespace BitPixel.Common.Users
{
	public interface IUserReader
	{
		Task<UserModel> GetUser(int id);
		Task<List<UserModel>> GetUsers();

		Task<UserProfileModel> GetUserProfile(string userName);
		Task<DataTablesResponseData> GetUsers(DataTablesParam model);
	}
}
