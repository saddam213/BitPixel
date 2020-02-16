using System.Threading.Tasks;

using DotMatrix.Common.Results;

namespace DotMatrix.Common.Prize
{
	public interface IPrizeWriter
	{
		Task<IWriterResult> CreatePrizePool(CreatePrizePoolModel model);
		Task<IWriterResult> UpdatePrizePool(UpdatePrizePoolModel model);
		Task<IWriterResult> ClaimPrize(int userId, ClaimPrizeModel model);
		Task<IWriterResult> UpdatePrizePayment(UpdatePrizePaymenModel model);
	}
}
