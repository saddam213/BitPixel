using System;
using System.ComponentModel.DataAnnotations;

using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class PaymentMethod
	{
		[Key]
		public int Id { get; set; }
		public PaymentMethodType Type { get; set; }
		public PaymentMethodStatus Status { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public string Description { get; set; }
		public string Note { get; set; }
		public decimal Rate { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data5 { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
