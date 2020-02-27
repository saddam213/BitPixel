using BitPixel.Enums;
using System;

namespace BitPixel.Common.Payment
{
	public class PaymentReceiptModel
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public decimal Amount { get; set; }
		public PaymentReceiptStatus Status { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Created { get; set; }
		public int Points { get; set; }
		public decimal Rate { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data5 { get; set; }
	}
}
