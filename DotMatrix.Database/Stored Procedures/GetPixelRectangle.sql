CREATE PROCEDURE[dbo].[GetPixelRectangle]
  @X INT
 ,@Y INT
 ,@Width INT
 ,@Height INT
AS

SET NOCOUNT ON;

SELECT DISTINCT 
	 RightBottom.[Id]
	,RightBottom.[UserId]
	,RightBottom.[X]
	,RightBottom.[Y]
	,RightBottom.[Color]
	,RightBottom.[Type]
	,RightBottom.[Points]
	,RightBottom.[LastUpdate]
FROM [dbo].[Pixel] AS LeftTop
JOIN [dbo].[Pixel] AS LeftBottom
   ON  LeftBottom.X = LeftTop.X
  AND LeftBottom.Y >= LeftTop.Y
JOIN [dbo].[Pixel] AS RightTop
    ON  RightTop.Y = LeftTop.Y
    AND RightTop.X >= LeftTop.X
JOIN [dbo].[Pixel] AS RightBottom
    ON  RightBottom.X = RightTop.X
    AND RightBottom.Y = LeftBottom.Y
WHERE LeftTop.X >= @X
AND RightBottom.X < @X + @Width
AND LeftTop.Y >= @Y
AND RightBottom.Y < @Y + @Height
