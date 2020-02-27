using System.Threading.Tasks;

using BitPixel.Common.Results;

namespace BitPixel.Common.Prize
{
	public interface IPrizeWriter
	{
		Task<IWriterResult> CreatePrizePool(CreatePrizePoolModel model);
		Task<IWriterResult> UpdatePrizePool(UpdatePrizePoolModel model);
		Task<IWriterResult> ClaimPrize(int userId, ClaimPrizeModel model);
		Task<IWriterResult> UpdatePrizePayment(UpdatePrizePaymenModel model);
	}
}
