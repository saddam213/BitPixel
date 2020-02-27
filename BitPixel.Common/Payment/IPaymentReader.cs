using System.Collections.Generic;
using System.Threading.Tasks;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;

namespace BitPixel.Common.Payment
{
	public interface IPaymentReader
	{
		Task<List<PaymentMethodModel>> GetMethods();
		Task<PaymentMethodModel> GetMethod(string symbol);

		Task<PaymentUserMethodModel> GetUserMethod(int userId, int paymentMethodId);

		Task<List<PaymentReceiptModel>> GetReceipts();
		Task<List<PaymentReceiptModel>> GetReceipts(int userId);
		Task<PaymentReceiptModel> GetReceipt(int paymentReceiptId);
		Task<PaymentReceiptModel> GetReceipt(int userId, int paymentReceiptId);
		Task<DataTablesResponseData> GetReceipts(DataTablesParam model);
	}
}
