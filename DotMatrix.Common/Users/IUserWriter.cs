using System.Collections.Generic;
using System.Threading.Tasks;
using DotMatrix.Common.Results;
using DotMatrix.Entity;

namespace DotMatrix.Common.Users
{
	public interface IUserWriter
	{
		Task<IWriterResult> UpdateUser(UpdateUserModal model);
	}
}
