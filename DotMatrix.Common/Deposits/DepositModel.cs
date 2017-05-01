using DotMatrix.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Deposits
{
	public class DepositModel
	{
		public decimal Amount { get; set; }
		public int Confirmations { get; set; }
		public int Id { get; set; }
		public DepositStatus Status { get; set; }
		public DateTime Timestamp { get; set; }
		public string TxId { get; set; }
	}
}
