CREATE PROCEDURE[dbo].[Cache_Pixel_GetInitial]
AS

	SELECT 
		 CONCAT(p.[GameId],'-', p.[X], '-', p.[Y]) AS [Id]
		,p.[X]
		,p.[Y]
		,p.[GameId]
		,p.[Type]
		,p.[Color]
		,p.[Points]
		,p.[LastUpdated]
	FROM [dbo].[Pixel] p
	JOIN [dbo].[Game] g ON g.[Id] = p.[GameId]
	WHERE g.[Status] NOT IN (3, 10) -- finished, complete