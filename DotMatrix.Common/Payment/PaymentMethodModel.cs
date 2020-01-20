using DotMatrix.Enums;
using System;

namespace DotMatrix.Common.Payment
{
	public class PaymentMethodModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public PaymentMethodType Type { get; set; }
		public PaymentMethodStatus Status { get; set; }
		public decimal Rate { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Created { get; set; }
		public string Symbol { get; set; }
		public string Note { get; set; }
	}
}
