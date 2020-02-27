CREATE PROCEDURE[dbo].[Cache_Pixel_GetUpdates]
  @LastUpdate TIMESTAMP
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
	WHERE p.[LastUpdated] > @LastUpdate