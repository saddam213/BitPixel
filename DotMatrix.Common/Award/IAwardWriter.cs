using System.Threading.Tasks;
using DotMatrix.Common.Results;

namespace DotMatrix.Common.Award
{
	public interface IAwardWriter
	{
		Task<IWriterResult> AddUserAward(AddUserAwardModel model);
		Task<IWriterResult> UpdateAward(UpdateAwardModel model);
	}
}
