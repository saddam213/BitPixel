using System;

using DotMatrix.Enums;

namespace DotMatrix.Common.Payment
{
	public class UpdatePaymentModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Created { get; set; }

		public int Points { get; set; }
		public decimal Rate { get; set; }
		public PaymentReceiptStatus Status { get; set; }
		public decimal Amount { get; set; }
		public string Data2 { get; set; }
	}
}
