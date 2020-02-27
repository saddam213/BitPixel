namespace DotMatrix.Common.Users
{
	public class UserModel
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public int Points { get; set; }
		public bool IsEmailConfirmed { get; set; }
		public bool IsLocked { get; set; }
	}
}
