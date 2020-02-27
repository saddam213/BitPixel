CREATE PROCEDURE[dbo].[Game_AddPixel]
  @GameId INT
 ,@UserId INT
 ,@X INT
 ,@Y INT
 ,@Type TINYINT
 ,@Color NVARCHAR(7)
 ,@Points INT
 ,@MaxPoints INT
 ,@GameType TINYINT
AS

	DECLARE @UserPoints INT;
	DECLARE @UserName NVARCHAR(128);

	DECLARE @TeamId INT;

	DECLARE @PixelId INT;
	DECLARE @PixelType TINYINT;
	DECLARE @PixelPoints INT;

	DECLARE @NewPixelPoints INT;
	DECLARE @Timestamp DATETIME2(7) = SYSUTCDATETIME();

	SELECT 
		@PixelId = p.[Id],
		@PixelType = p.[Type],
		@PixelPoints = p.[Points]
	FROM [dbo].[Pixel] p
	WHERE [GameId] = @GameId
	AND [X] = @X AND [Y] = @Y

	IF(@PixelId IS NOT NULL AND @PixelType = 2)-- Fixed
	BEGIN
		SELECT 'Cannot overwrite gameboard pixel' AS [Error]
		RETURN;
	END

	SELECT 
		@UserPoints = u.[Points],
		@UserName = u.[UserName]
	FROM [dbo].[Users] u
	WHERE u.[Id] = @UserId

	IF(@GameType = 1)-- TeamBattle
	BEGIN
			SELECT 
				@TeamId = t.[Id],
				@Color = t.Color
			FROM [dbo].[TeamMember] tm 
			JOIN [dbo].[Team] t ON t.[Id] = tm.[TeamId]
			WHERE t.[GameId] = @GameId 
			AND tm.[UserId] = @UserId

			IF(@TeamId IS NULL)
			BEGIN
				SELECT 'Please select a team to play' AS [Error]
				RETURN;
			END
	END


	SELECT @PixelPoints = ISNULL(@PixelPoints, @Points);

	IF(@UserPoints < @PixelPoints)
	BEGIN
		SELECT 'Insufficient points to create pixel' AS [Error]
		RETURN;
	END

	IF(@PixelPoints > @MaxPoints)
	BEGIN
		SELECT 'Pixel points are greater than Spend Limit' AS [Error]
		RETURN;
	END

	IF(@PixelId IS NULL)
		BEGIN
			SELECT @NewPixelPoints = @Points + @Points;
			INSERT INTO [dbo].[Pixel]([GameId], [UserId], [TeamId], [X], [Y], [Type], [Color], [Points], [LastUpdate])
			VALUES(@GameId, @UserId, @TeamId, @X, @Y, @Type, @Color, @NewPixelPoints, @Timestamp)
			SELECT @PixelId = SCOPE_IDENTITY()
		END
	ELSE
		BEGIN
			SELECT @NewPixelPoints = @PixelPoints + @PixelPoints;
			UPDATE [dbo].[Pixel]
			SET [UserId] = @UserId
				,[TeamId] = @TeamId
				,[Type] = @Type
				,[Color] = @Color
				,[Points] = @NewPixelPoints
				,[LastUpdate] = @Timestamp
			WHERE [Id] = @PixelId
		END

	INSERT INTO [dbo].[PixelHistory]([PixelId], [GameId], [UserId], [Type], [Color], [Points],[TeamId], [Timestamp])
	VALUES(@PixelId, @GameId, @UserId, @Type, @Color, @PixelPoints, @TeamId, @Timestamp)

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
		@NewPixelPoints AS [NewPoints],
		@UserPoints AS [UserPoints]
	