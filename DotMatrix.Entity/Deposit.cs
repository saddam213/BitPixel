using DotMatrix.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Entity
{
	public class Deposit
	{
		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
		public string TxId { get; set; }
		public string BlockHash { get; set; }
		public decimal Amount { get; set; }
		public int Confirmations { get; set; }
		public DepositStatus Status { get; set; }
		public DateTime Timestamp { get; set; }

		public virtual User User { get; set; }

	}
}
