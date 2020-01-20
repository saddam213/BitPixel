using System;
using System.ComponentModel.DataAnnotations;

namespace DotMatrix.Entity
{
	public class PaymentAddress
	{
		[Key]
		public int Id { get; set; }
		public int PaymentMethodId { get; set; }
		public int? UserId { get; set; }
		public string Address { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Timestamp { get; set; }

		public virtual PaymentMethod PaymentMethod { get; set; }
	}
}
