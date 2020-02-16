using System.Threading.Tasks;
using DotMatrix.Common.Results;

namespace DotMatrix.Common.Payment
{
	public interface IPaymentWriter
	{
		Task<IWriterResult> CreateMethod(int userId, int paymentMethodId);
		Task<IWriterResult> UpdatePayment(UpdatePaymentModel model);
	}
}
