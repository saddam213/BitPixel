DECLARE @Current DATETIME2(7) = SYSUTCDATETIME();

DECLARE @SystemUserId INT = 1;
DECLARE @DefaultGameId INT = 1;
DECLARE @DefaultTeamId INT = 1;
DECLARE @AdminRoleId INT = 1;


-- Game
SET IDENTITY_INSERT [dbo].[Game] ON 
INSERT [dbo].[Game] ([Id], [Name],[Description], [Type],[Status],[Width],[Height],[Rank],[Timestamp]) 
VALUES (@DefaultGameId, N'Game 1', N'', 0, 0, 800, 600, 0, @Current)
SET IDENTITY_INSERT [dbo].[Game] OFF


-- Team
SET IDENTITY_INSERT [dbo].[Team] ON 
INSERT [dbo].[Team] ([Id], [Name]) VALUES (@DefaultTeamId, N'No Team')
SET IDENTITY_INSERT [dbo].[Team] OFF


-- Users
SET IDENTITY_INSERT [dbo].[Users] ON 
INSERT [dbo].[Users] ([Id], [UserName], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [TeamId], [IsApiEnabled], [ApiKey], [ApiSecret], [Points]) 
VALUES 
 (@SystemUserId, N'System',	N'System@bitpixel.chainstack.nz', 1, N'AJeijVxBCaOH3r80aPR9yzc/aLGZYw87XJSZDjcApDd14LtWbj50bUBSJch45Onyww==', N'EAC481E0-258B-4BFC-BA6C-FB74360D8A82',NULL, 1, 0, @DefaultTeamId, 0,'','', 0)
SET IDENTITY_INSERT [dbo].[Users] OFF


-- Roles
SET IDENTITY_INSERT [dbo].[UserRoles] ON 
INSERT [dbo].[UserRoles] ([Id], [Name]) VALUES (@AdminRoleId, N'Admin')
SET IDENTITY_INSERT [dbo].[UserRoles] OFF

SET IDENTITY_INSERT [dbo].[UserInRoles] ON
INSERT [dbo].[UserInRoles] ([UserId], [RoleId]) VALUES (@SystemUserId, @AdminRoleId)
SET IDENTITY_INSERT [dbo].[UserInRoles] OFF

INSERT INTO [dbo].[EmailTemplate] ([Type], [Culture], [Parameters],[FromAddress],[Subject],[Template], [TemplateHtml], [IsEnabled])
Values
(
	0, --Registration
	'en-US',	
	'{0} = UserName, {1} = Activation Link',
	'noreply@bitpixel.chainstack.nz',
	'Registration',
	'{0} = UserName, {1} = Activation Link',
	'{0} = UserName, {1} = Activation Link',
	1
),
(
	1, --ResetPassword
	'en-US',	
	'{0} = UserName, {1} = Reset Link',
	'noreply@bitpixel.chainstack.nz',
	'Reset Password',
	'{0} = UserName, {1} = Reset Link',
	'{0} = UserName, {1} = Reset Link',
	1
)
GO

