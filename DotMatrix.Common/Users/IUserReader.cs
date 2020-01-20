using System.Collections.Generic;
using System.Threading.Tasks;

using DotMatrix.Entity;

namespace DotMatrix.Common.Users
{
	public interface IUserReader
	{
		Task<User> GetUser(int id);
		Task<List<User>> GetUsers();
	}
}
