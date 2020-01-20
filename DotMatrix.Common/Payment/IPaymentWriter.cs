using System.Threading.Tasks;

namespace DotMatrix.Common.Payment
{
	public interface IPaymentWriter
	{
		Task<bool> CreateMethod(int userId, int paymentMethodId);
	}
}
