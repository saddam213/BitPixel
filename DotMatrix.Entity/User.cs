using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotMatrix.Entity
{
	public class User
	{
		public int Id { get; set; }
		public int TeamId { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public bool IsApiEnabled { get; set; }
		public int Points { get; set; }

		[ForeignKey("TeamId")]
		public Team Team { get; set; }

		public virtual ICollection<PaymentReceipt> Deposits { get; set; }
	}
}
