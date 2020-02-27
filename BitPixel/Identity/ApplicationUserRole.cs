using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BitPixel.Identity
{
	public class ApplicationUserRole : IdentityUserRole<int>
	{
		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("RoleId")]
		public virtual ApplicationRole Role { get; set; }
	}
}