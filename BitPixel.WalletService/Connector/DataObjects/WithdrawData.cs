using System.Runtime.Serialization;

namespace BitPixel.WalletService.Connector.DataObjects
{
	/// <summary>
	/// Withdraw rpc data response object
	/// </summary>
	[DataContract]
	public class WithdrawData
	{
		/// <summary>
		/// Gets or sets the txid.
		/// </summary>
		[DataMember(Name = "txid")]
		public string Txid { get; set; }
	}
}
