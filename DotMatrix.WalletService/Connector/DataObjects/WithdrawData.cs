using System.Runtime.Serialization;

namespace DotMatrix.WalletService.Connector.DataObjects
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
