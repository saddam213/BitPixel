SET IDENTITY_INSERT [dbo].[Award] ON 
GO

DECLARE @Timestamp DATETIME2(7) = GETUTCDATE();

INSERT [dbo].[Award] (
[Id],	[Name],					        [Icon],					        [Type],   [TriggerType],[Status],	[ClickType],[Rank], [Points],		[Level], [Description], [Timestamp]) 
VALUES 
 (1,	N'Registration',		    N'fa fa-user-check',	        1,		      1,				0,			  10, 			0,		  250,			1,		    NULL,			@Timestamp)
,(2,	N'Portrait',		        N'far fa-portrait',	          2,		      1,				0,			  10, 			0,		  100,			1,		    NULL,			@Timestamp)

,(100,	N'Best Picture',			N'fa fa-camera',	            100,	      3,				0,			  10,				0,		  10000,		10,		    NULL,			@Timestamp)
,(200,	N'Best Animation',	  N'fa fa-camera-movie',        200,	      3,				0,			  10,				0,		  25000,		10,		    NULL,			@Timestamp)

,(1000,	N'Daily Click',			  N'fa fa-calendar-day',	      1000,	      2,				0,			  0,				0,		  25,				0,		    NULL,			@Timestamp)
,(1001,	N'100th Click',			  N'fa fa-mouse-pointer',       1001,	      1,				0,			  0,				0,		  25,				0,		    NULL,			@Timestamp)
,(1002,	N'1000th Click',		  N'fa fa-mouse-pointer',       1002,	      1,				0,			  0,				0,		  50,				0,		    NULL,			@Timestamp)
,(1003,	N'10000th Click',		  N'fa fa-mouse-pointer',	      1003,	      1,				0,			  0,				0,		  100,			0,		    NULL,			@Timestamp)
,(1004,	N'100000th Click',		N'fa fa-mouse-pointer',	      1004,	      1,				0,			  0,				0,		  5000,			1,		    NULL,			@Timestamp)
,(1005,	N'1000000th Click',		N'fa fa-mouse-pointer',	      1005,	      1,				0,			  0,				0,		  25000,		1,		    NULL,			@Timestamp)
,(1006,	N'Four Corners',		  N'fa fa-th-large',		        1006,	      4,				0,			  0,				0,		  100,			0,		    NULL,			@Timestamp)
,(1007,	N'Vertical Line',		  N'fa fa-arrows-alt-v',	      1007,	      5,				0,			  0,				0,		  200,			1,		    NULL,			@Timestamp)
,(1008,	N'Horizontal Line',		N'fa fa-arrows-alt-h',	      1008,	      5,				0,			  0,				0,		  200,			1,		    NULL,			@Timestamp)
,(1009,	N'Border Line',		    N'fa fa-border-outer',	      1009,	      4,				0,			  0,				0,		  200,			1,		    NULL,			@Timestamp)


,(2000,	N'Daily Pixel',			  N'fa fa-calendar-day',        2000,	      2,				0,			  1,				0,		  50,				0,		    NULL,			@Timestamp)
,(2001,	N'100th Pixel',			  N'fa fa-paint-brush',         2001,	      1,				0,		  	1,				0,		  50,				0,		    NULL,			@Timestamp)
,(2002,	N'1000th Pixel',		  N'fa fa-paint-brush',         2002,	      1,				0,		  	1,				0,		  100,			0,		    NULL,			@Timestamp)
,(2003,	N'10000th Pixel',		  N'fa fa-paint-brush',         2003,	      1,				0,		  	1,				0,		  200,			1,		    NULL,			@Timestamp)
,(2004,	N'100000th Pixel',		N'fa fa-paint-brush',         2004,	      1,				0,		  	1,				0,		  10000,		2,		    NULL,			@Timestamp)
,(2005,	N'1000000th Pixel',		N'fa fa-paint-brush',         2005,	      1,				0,			  1,				0,		  50000,		2,		    NULL,			@Timestamp)
,(2006,	N'Four Corners',		  N'fa fa-th-large',            2006,	      4,				0,			  1,				0,		  200,			0,		    NULL,			@Timestamp)
,(2007,	N'Vertical Line',		  N'fa fa-arrows-alt-v',        2007,	      5,				0,			  1,				0,		  400,			2,		    NULL,			@Timestamp)
,(2008,	N'Horizontal Line',		N'fa fa-arrows-alt-h',        2008,	      5,				0,			  1,				0,		  400,			2,		    NULL,			@Timestamp)
,(2009,	N'Border Line',		    N'fa fa-border-outer',        2009,	      4,				0,			  1,				0,		  200,			2,		    NULL,			@Timestamp)
,(2010,	N'Last Pixel',		    N'fal fa-flag-checkered',     2010,	      3,			  0,			  1,				0,		  25000,		10,		    NULL,			@Timestamp)

,(2100,	N'Deface',			      N'fa fa-bring-forward',	      2100,	      1,				0,			  1,				0,		  5,				0,		    NULL,			@Timestamp)
,(2101,	N'Hoodlum',			      N'fa fa-mask',	              2101,	      1,				0,			  1,				0,		  50,				0,		    NULL,			@Timestamp)
,(2102,	N'Vandal',		        N'fa fa-spray-can',           2102,	      1,				0,			  1,				0,		  100,			1,		    NULL,			@Timestamp)
,(2103,	N'Destroyer',		      N'fa fa-space-station-moon-alt', 2103,    1,				0,			  1,				0,		  200,			1,		    NULL,			@Timestamp)
,(2104,	N'Anarchist',		      N'fa fa-hand-middle-finger',	2104,	      1,				0,			  1,				0,		  10000,		2,		    NULL,			@Timestamp)
,(2105,	N'Super Villan',		  N'fa fa-helmet-battle',	      2105,	      1,				0,			  1,				0,		  50000,		2,		    NULL,			@Timestamp)

,(3000,	N'Lucky 420',			    N'fa fa-cannabis',		        3000,	      2,				0,			  0,				0,		  42,				0,		    NULL,			@Timestamp)
,(3001,	N'Nice',				      N'fa fa-thumbs-up',		        3001,	      2,				0,			  0,				0,		  69,				0,		    NULL,			@Timestamp)
,(3002,	N'Inside Job',	      N'fa fa-building',		        3002,	      2,				0,			  0,				0,		  93,				0,		    NULL,		  @Timestamp)

,(4000,	N'RGB',				        N'fad fa-brush',		          4000,	      4,				0,			  1,				0,		  10,				0,		    NULL,			@Timestamp)
,(4001,	N'Color Blind',				N'far fa-blind',		          4001,	      4,				0,			  1,				0,		  20,				0,		    NULL,			@Timestamp)
,(4002,	N'Rainbow',				    N'fa fa-rainbow',		          4002,	      4,				0,			  1,				0,		  100,			0,		    NULL,			@Timestamp)

,(4003,	N'Colorful',				  N'fal fa-swatchbook',		      4003,	      4,				0,			  1,				0,		  20,			  0,		    NULL,			@Timestamp)
,(4004,	N'Palette',				    N'far fa-palette',		        4004,	      4,				0,			  1,				0,		  200,			1,		    NULL,			@Timestamp)
,(4005,	N'Kaleidoscope',			N'fad fa-certificate',	      4005,	      4,				0,			  1,				0,		  2000,			2,		    NULL,			@Timestamp)
GO
SET IDENTITY_INSERT [dbo].[Award] OFF
GO 