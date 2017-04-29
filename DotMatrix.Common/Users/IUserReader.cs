using DotMatrix.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Users
{
	public interface IUserReader
	{
		Task<User> GetUser(string id);
		Task<List<User>> GetUsers();
	}
}
