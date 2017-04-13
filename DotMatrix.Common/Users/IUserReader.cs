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
		Task<List<User>> GetUsers();
	}
}
