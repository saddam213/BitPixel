CREATE PROCEDURE[dbo].[Cache_Game_GetInitial]
AS

	SELECT 
		 t.[Id]
		,t.[Name]
		,t.[Description]
		,t.[Type]
		,t.[Status]
		,t.[Platform]
		,t.[Width]
		,t.[Height]
		,t.[ClicksPerSecond]
		,t.[Rank]
		,t.[Timestamp]
		,t.[LastUpdated]
	FROM [dbo].[Game] t