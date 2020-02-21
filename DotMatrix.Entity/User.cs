using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotMatrix.Entity
{
	public class User
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public bool IsApiEnabled { get; set; }
		public int Points { get; set; }
		public string SecurityStamp { get; set; }
		public bool EmailConfirmed { get; set; }
		public DateTime? LockoutEndDateUtc { get; set; }

		public virtual ICollection<Click> Clicks { get; set; }
		public virtual ICollection<Prize> Prizes { get; set; }
		public virtual ICollection<PixelHistory> Pixels { get; set; }
		public virtual ICollection<AwardHistory> Awards { get; set; }

		public virtual ICollection<PaymentReceipt> Deposits { get; set; }
	}
}
