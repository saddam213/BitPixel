using System.Collections.Generic;
using System.Threading.Tasks;
using BitPixel.Common.Results;
using BitPixel.Entity;

namespace BitPixel.Common.Users
{
	public interface IUserWriter
	{
		Task<IWriterResult> UpdateUser(UpdateUserModal model);
	}
}
