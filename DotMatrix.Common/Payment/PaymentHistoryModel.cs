using System.Collections.Generic;

namespace DotMatrix.Common.Payment
{
	public class PaymentHistoryModel
	{
		public List<PaymentReceiptModel> Receipts { get; set; } = new List<PaymentReceiptModel>();
	}
}
