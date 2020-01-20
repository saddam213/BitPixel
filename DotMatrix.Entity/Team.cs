using System.Collections.Generic;

namespace DotMatrix.Entity
{
	public class Team
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<User> Users { get; set; }
	}
}
