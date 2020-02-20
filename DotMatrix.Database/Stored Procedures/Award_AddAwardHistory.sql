CREATE PROCEDURE [dbo].[Award_AddAwardHistory]
  @GameId INT NULL
 ,@UserId INT
 ,@Type SMALLINT
 ,@Version NVARCHAR(128)
 ,@VersionData NVARCHAR(50)
AS

  SET NOCOUNT ON;

  DECLARE @UserPoints INT;
  DECLARE @AwardId INT;
  DECLARE @AwardLevel TINYINT;
  DECLARE @AwardName NVARCHAR(50);
  DECLARE @AwardPoints INT = 0;
  DECLARE @AwardHistoryId INT;
  DECLARE @Timestamp DATETIME2(7) = SYSUTCDATETIME();

  SELECT
    @AwardId = [Id],
    @AwardPoints = [Points],
    @AwardName = [Name],
    @AwardLevel = [Level]
  FROM [dbo].[Award]
  WHERE [Type] = @Type
  AND [Status] = 0 --Active

  IF(@AwardId IS NULL)
  BEGIN
    SELECT 'Award not found or is no longer active' AS [Error]
    RETURN;
  END

  INSERT INTO [dbo].[AwardHistory] ([AwardId], [UserId], [GameId], [Points], [Version],[VersionData], [Timestamp])
  SELECT @AwardId, @UserId, @GameId, @AwardPoints, @Version, @VersionData, @Timestamp
  SELECT @AwardHistoryId = SCOPE_IDENTITY();

  EXEC @UserPoints = [dbo].[User_AuditPoints] @UserId

  SELECT 
     @UserId AS [UserId]
    ,[UserName] AS [UserName]
    ,@AwardHistoryId AS [AwardId]
    ,@AwardName AS [AwardName]
    ,@AwardPoints AS [AwardPoints]
    ,@AwardLevel AS [AwardLevel]
    ,@UserPoints AS [UserPoints]
  FROM [dbo].[Users]
  WHERE [Id] = @UserId