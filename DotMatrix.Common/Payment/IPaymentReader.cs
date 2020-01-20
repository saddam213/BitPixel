using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotMatrix.Common.Payment
{
	public interface IPaymentReader
	{
		Task<List<PaymentMethodModel>> GetMethods();
		Task<PaymentUserMethodModel> GetMethod(int userId, int paymentMethodId);

		Task<List<PaymentReceiptModel>> GetReceipts(int userId);
		Task<PaymentReceiptModel> GetReceipt(int userId, int paymentReceiptId);
	}
}
