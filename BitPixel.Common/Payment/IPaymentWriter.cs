using System.Threading.Tasks;
using BitPixel.Common.Results;

namespace BitPixel.Common.Payment
{
	public interface IPaymentWriter
	{
		Task<IWriterResult> CreateMethod(int userId, int paymentMethodId);
		Task<IWriterResult> UpdatePayment(UpdatePaymentModel model);
	}
}
