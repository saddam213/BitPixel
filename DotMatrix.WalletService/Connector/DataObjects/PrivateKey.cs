using System.Runtime.Serialization;

namespace DotMatrix.WalletService.Connector.DataObjects
{
	[DataContract]
	public class PrivateKeyData
	{
		[DataMember(Name = "Private Key: ")]
		public string PrivateKey { get; set; }

		[DataMember(Name = "WARNING:")]
		public string Warning { get; set; }
	}
}
