using System.ComponentModel.DataAnnotations;

namespace DotMatrix.Common.Account
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
