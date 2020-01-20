namespace DotMatrix.WalletService.Connector.DataObjects
{
	/// <summary>
	/// The transaction type
	/// </summary>
	public enum TransactionDataType
	{
		All,
		Deposit,
		Withdraw,
		Transfer,
		Mined,
		Immature,
		Orphan,
		Savings
	}
}