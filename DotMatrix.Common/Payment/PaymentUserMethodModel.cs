using DotMatrix.Enums;
using System;

namespace DotMatrix.Common.Payment
{
	public class PaymentUserMethodModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Note { get; set; }
		public PaymentMethodType MethodType { get; set; }
		public PaymentUserMethodStatus Status { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data5 { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Created { get; set; }
	}
}
