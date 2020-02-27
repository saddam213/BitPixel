using System.Threading.Tasks;
using BitPixel.Common.Results;

namespace BitPixel.Common.Award
{
	public interface IAwardWriter
	{
		Task<IWriterResult> AddUserAward(AddUserAwardModel model);
		Task<IWriterResult> UpdateAward(UpdateAwardModel model);
	}
}
