CREATE PROCEDURE [dbo].[Game_AddClick]
  @GameId INT
 ,@UserId INT
 ,@Type TINYINT
 ,@X INT
 ,@Y INT
AS

  SET NOCOUNT ON;

  DECLARE @PrizeId INT;
  DECLARE @PrizeName NVARCHAR(50);
  DECLARE @PrizeDescription NVARCHAR(128);
  DECLARE @PrizePoints INT = 0;
  DECLARE @UserPoints INT = 0;
  DECLARE @Timestamp DATETIME2(7) = SYSUTCDATETIME();

  INSERT INTO [dbo].[Click] ([GameId], [UserId], [Type], [X], [Y], [Timestamp])
  SELECT @GameId, @UserId, @Type, @X, @Y, @Timestamp

  IF(@@ROWCOUNT = 1 AND @Type = 1) -- AddPixel
  BEGIN
    UPDATE [dbo].[Prize]
       SET [UserId] = @UserId
          ,[IsClaimed] = 1
          ,[ClaimTime] = @Timestamp
          ,[Status] = IIF([Type] = 0, 10, 0)--complete or unclaimed
          ,@PrizeId = [Id]
          ,@PrizeName = [Name]
          ,@PrizePoints = [Points]
          ,@PrizeDescription = [Description]
    WHERE [GameId] = @GameId
    AND [IsClaimed] = 0 
    AND [X] = @X AND [Y] = @Y

    IF(@PrizeId IS NOT NULL)
      BEGIN
        EXEC @UserPoints = [dbo].[User_AuditPoints] @UserId
      END
  END

  SELECT SCOPE_IDENTITY() AS [ClickId]
        ,@PrizeId AS [PrizeId]
        ,@PrizeName AS [PrizeName]
        ,@PrizePoints AS [PrizePoints]
        ,@PrizeDescription AS [PrizeDescription]
        ,@UserPoints AS [UserPoints]