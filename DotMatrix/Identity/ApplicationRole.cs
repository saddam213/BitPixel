using Microsoft.AspNet.Identity.EntityFramework;

namespace DotMatrix.Identity
{
	public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
	{
		public ApplicationRole() { }
		public ApplicationRole(string name) { Name = name; }
	}
}