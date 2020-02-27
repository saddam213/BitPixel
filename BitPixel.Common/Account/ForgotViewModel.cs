using System.ComponentModel.DataAnnotations;

namespace BitPixel.Common.Account
{
	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
