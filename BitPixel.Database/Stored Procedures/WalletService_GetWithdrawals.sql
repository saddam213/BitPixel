CREATE PROCEDURE [dbo].[WalletService_GetWithdrawals]
AS

	SET NOCOUNT ON;

	DECLARE @Timestamp DATETIME2(7) =  SYSDATETIMEOFFSET();
	DECLARE @Processing TABLE
	(
		[Id] INT,
		[Data] NVARCHAR(256),
		[Data2] NVARCHAR(256),
		[Data3] NVARCHAR(256)
	);

	UPDATE [dbo].[Prize]
	SET [Status] = 2 -- Processing
	OUTPUT inserted.[Id],
				 inserted.[Data] ,
				 inserted.[Data2],
				 inserted.[Data3]
	INTO @Processing
	WHERE [IsClaimed] = 1
	AND [Type] = 1   -- Crypto
	AND [Status] = 1 -- Pending

	SELECT 
		p.[Id] AS [PrizeId],
		p.[Data] AS [Symbol],
		p.[Data2] AS [Amount],
		p.[Data3] AS [Destination],
		pm.[Data] AS [Host],
		pm.[Data2] AS [UserName],
		pm.[Data3] AS [Password]
	FROM @Processing p
	LEFT JOIN [PaymentMethod] pm ON pm.[Symbol] = p.[Data]