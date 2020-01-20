namespace DotMatrix.Common.Api
{
	public class UserApiKeyPair
	{
		public string Secret { get; set; }
		public string Key { get; set; }

		public bool IsValid
		{
			get { return !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Key); }
		}
	}
}
