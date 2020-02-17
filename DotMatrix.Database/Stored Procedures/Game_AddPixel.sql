CREATE PROCEDURE[dbo].[Game_AddPixel]
  @GameId INT
 ,@UserId INT
 ,@X INT
 ,@Y INT
 ,@Type TINYINT
 ,@Color NVARCHAR(7)
 ,@Points INT
 ,@MaxPoints INT
AS

	DECLARE @TeamId INT;
	DECLARE @UserPoints INT;
	DECLARE @UserName NVARCHAR(128);
	DECLARE @TeamName NVARCHAR(128);

	DECLARE @PixelId INT;
	DECLARE @PixelType TINYINT;
	DECLARE @PixelPoints INT;

	DECLARE @NewPixelPoints INT;
	DECLARE @Timestamp DATETIME2(7) = SYSUTCDATETIME();

	SELECT 
		@PixelId = [Id],
		@PixelType = [Type],
		@PixelPoints = [Points]
	FROM [dbo].[Pixel]
	WHERE [GameId] = @GameId
	AND [X] = @X AND [Y] = @Y

	IF(@PixelId IS NOT NULL AND @PixelType = 2)-- Fixed
	BEGIN
		SELECT 'Cannot overwrite fixed pixel' AS [Error]
		RETURN;
	END

	SELECT 
		@UserPoints = u.[Points],
		@UserName = u.[UserName],
		@TeamId = t.[Id],
		@TeamName = t.[Name]
	FROM [dbo].[Users] u
	JOIN [dbo].[Team] t ON t.[Id] = u.[TeamId]
	WHERE u.[Id] = @UserId

	SELECT @PixelPoints = ISNULL(@PixelPoints, @Points);

	IF(@UserPoints < @PixelPoints)
	BEGIN
		SELECT 'Insufficient points to set pixel' AS [Error]
		RETURN;
	END

	IF(@PixelPoints > @MaxPoints)
	BEGIN
		SELECT 'Points are greater than MaxPoints' AS [Error]
		RETURN;
	END

	IF(@PixelId IS NULL)
		BEGIN
			SELECT @NewPixelPoints = @Points + @Points;
			INSERT INTO [dbo].[Pixel]([GameId], [UserId], [X], [Y], [Type], [Color], [Points], [LastUpdate])
			VALUES(@GameId, @UserId, @X, @Y, @Type, @Color, @NewPixelPoints, @Timestamp)
			SELECT @PixelId = SCOPE_IDENTITY()
		END
	ELSE
		BEGIN
			SELECT @NewPixelPoints = @PixelPoints + @PixelPoints;
			UPDATE [dbo].[Pixel]
			SET [UserId] = @UserId
				,[Type] = @Type
				,[Color] = @Color
				,[Points] = @NewPixelPoints
				,[LastUpdate] = @Timestamp
			WHERE [Id] = @PixelId
		END

	INSERT INTO [dbo].[PixelHistory]([PixelId], [GameId], [UserId], [Type], [Color], [Points], [Timestamp])
	VALUES(@PixelId, @GameId, @UserId, @Type, @Color, @PixelPoints, @Timestamp)

	--EXEC @UserPoints = [dbo].[User_AuditPoints] @UserId
	UPDATE [dbo].[Users]
		SET @UserPoints = [Points]
				,[Points] = [Points] - @PixelPoints
	WHERE [Id] = @UserId
	SELECT @UserPoints = @UserPoints - @PixelPoints;


	SELECT 
		@PixelId AS [PixelId], 
		@UserId AS [UserId],
		@UserName AS [UserName], 
		@TeamId AS [TeamId],
		@TeamName AS [TeamName],
		@NewPixelPoints AS [NewPoints],
		@UserPoints AS [UserPoints]
	