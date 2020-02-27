using System;
using System.ComponentModel.DataAnnotations;

using BitPixel.Enums;

namespace BitPixel.Entity
{
	public class PaymentUserMethod
	{
		[Key]
		public int Id { get; set; }
		public int UserId { get; set; }
		public int PaymentMethodId { get; set; }
		public PaymentUserMethodStatus Status { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data5 { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Timestamp { get; set; }

		public virtual User User { get; set; }
		public virtual PaymentMethod PaymentMethod { get; set; }
	}

}
