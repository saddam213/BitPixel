using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Entity
{
	public class User
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public bool IsApiEnabled { get; set; }
		public decimal Balance { get; set; }
		public string Address { get; set; }

		public virtual ICollection<Deposit> Deposits { get; set; }
	
	}
}
