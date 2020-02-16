CREATE PROCEDURE [dbo].[WalletService_UpdateWithdraw]
	 @PrizeId INT
	,@TransactionId NVARCHAR(256)
AS
	SET NOCOUNT ON;

	UPDATE [dbo].[Prize]
	SET  [Status] = 10 -- Claimed
			,[Data4] = @TransactionId
	WHERE [Id] = @PrizeId
