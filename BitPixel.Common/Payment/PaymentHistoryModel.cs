using System.Collections.Generic;

namespace BitPixel.Common.Payment
{
	public class PaymentHistoryModel
	{
		public List<PaymentReceiptModel> Receipts { get; set; } = new List<PaymentReceiptModel>();
	}
}
