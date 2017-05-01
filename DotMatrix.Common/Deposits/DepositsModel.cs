using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Deposits
{
	public class DepositsModel
	{
		public string Address { get; set; }
		public decimal Balance { get; set; }
		public List<DepositModel> Deposits { get; set; }
	}
}
