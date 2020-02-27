CREATE PROCEDURE[dbo].[Payment_AssignAddress]
  @UserId INT
 ,@PaymentMethodId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Processing TABLE
	(
		[Id]        INT,
		[UserId]    INT,
		[PaymentMethodId] INT ,
		[Address]   NVARCHAR(256),
		[Updated]   DATETIME2(7),
		[TimeStamp] DATETIME2(7)
	)

	--Check for existing address
	INSERT INTO @Processing([Id],  [Address])
	SELECT [Id], [Address]
	FROM [dbo].[PaymentAddress]
	WHERE  [UserId] = @UserId
	AND [PaymentMethodId] = @PaymentMethodId

	IF NOT EXISTS (SELECT * FROM @Processing)
	BEGIN
		UPDATE w
		 SET w.[UserId] = @UserId
			,w.[Updated] = SYSUTCDATETIME()
		OUTPUT
			 inserted.[Id]
			,inserted.[UserId]
			,inserted.[PaymentMethodId]
			,inserted.[Address]
			,inserted.[Updated]
			,inserted.[TimeStamp]
		INTO @Processing
		FROM [dbo].[PaymentAddress] w
		WHERE w.[UserId] IS NULL
		AND w.[Id] = 
		(
			SELECT TOP 1 [Id]
			FROM [dbo].[PaymentAddress]
			WHERE [PaymentMethodId] = @PaymentMethodId
			AND [UserId] IS NULL
		)
	END

	SELECT TOP 1 * FROM @Processing
END