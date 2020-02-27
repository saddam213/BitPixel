CREATE PROCEDURE [dbo].[User_AuditPoints]
  @UserId INT
AS

BEGIN

	DECLARE @PaymentsIN INT;
	DECLARE @PrizesIN INT;
	DECLARE @AwardsIN INT;
	DECLARE @PixelsOUT INT;
	DECLARE @PointsTotal INT;

	SELECT 
		@PaymentsIN = SUM([Points]) 
	FROM [dbo].[PaymentReceipt]
	WHERE [UserId] = @UserId
	AND [Status] = 2 -- Complete

	SELECT 
		@PrizesIN = SUM([Points]) 
	FROM [dbo].[Prize]
	WHERE [UserId] = @UserId
	AND [IsClaimed] = 1 -- Claimed

	SELECT 
		@AwardsIN = SUM([Points]) 
	FROM [dbo].[AwardHistory]
	WHERE [UserId] = @UserId

	SELECT 
		@PixelsOUT = SUM([Points])  
	FROM [dbo].[PixelHistory]
	WHERE [UserId] = @UserId

	SELECT @PointsTotal = (ISNULL(@PaymentsIN, 0) + ISNULL(@PrizesIN, 0)+ ISNULL(@AwardsIN, 0)) - ISNULL(@PixelsOUT, 0);

	UPDATE [dbo].[Users]
		SET [Points] = @PointsTotal
	WHERE [Id] = @UserId

	RETURN @PointsTotal

END