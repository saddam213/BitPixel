using Microsoft.AspNet.Identity.EntityFramework;

namespace BitPixel.Identity
{
	public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
	{
		public ApplicationRole() { }
		public ApplicationRole(string name) { Name = name; }
	}
}