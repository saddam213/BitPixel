using System.ComponentModel.DataAnnotations;

namespace BitPixel.Common.Users
{
	public class UpdateUserModal
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string UserName { get; set; }

		[Required]
		[EmailAddress]
		[MaxLength(50)]
		public string Email { get; set; }

		public int Points { get; set; }
		public bool IsApiEnabled { get; set; }
		public bool IsLocked { get; set; }
		public bool IsEmailConfirmed { get; set; }
	}
}
