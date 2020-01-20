namespace DotMatrix.Common.Api
{
	public class UserApiAuthKey
	{
		public UserApiAuthKey()
		{
		}

		public UserApiAuthKey(int userId, string key, string secret, bool isenabled)
		{
			Key = key;
			UserId = userId;
			Secret = secret;
			IsEnabled = isenabled;
		}

		public int UserId { get; set; }
		public string Secret { get; set; }
		public string Key { get; set; }
		public bool IsEnabled { get; set; }

		public bool IsValid
		{
			get { return !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Key); }
		}
	}
}
