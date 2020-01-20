DECLARE @Current DATETIME2(7) = SYSUTCDATETIME();

DECLARE @SystemUserId INT = 1;
DECLARE @DefaultTeamId INT = 1;
DECLARE @AdminRoleId INT = 1;

-- Team
SET IDENTITY_INSERT [dbo].[Team] ON 
INSERT [dbo].[Team] ([Id], [Name]) VALUES (@DefaultTeamId, N'No Team')
SET IDENTITY_INSERT [dbo].[Team] OFF
GO

-- Users
SET IDENTITY_INSERT [dbo].[Users] ON 
INSERT [dbo].[Users] ([Id], [UserName], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [TeamId], [IsApiEnabled], [ApiKey], [ApiSecret], [Points]) 
VALUES 
 (@SystemUserId, N'System',	N'System@bitpixel.chainstack.nz', 0, N'AJeijVxBCaOH3r80aPR9yzc/aLGZYw87XJSZDjcApDd14LtWbj50bUBSJch45Onyww==', N'EAC481E0-258B-4BFC-BA6C-FB74360D8A82',NULL, 1, 0, @DefaultTeamId, 0,'','', 0)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO

-- Roles
INSERT [dbo].[UserRoles] ([Id], [Name]) VALUES (@AdminRoleId, N'Admin')
INSERT [dbo].[UserInRoles] ([UserId], [RoleId]) VALUES (@SystemUserId, @AdminRoleId)
GO

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

-- Awards
SET IDENTITY_INSERT [dbo].[Award] ON 
INSERT [dbo].[Award] ([Id], [Name], [Icon], [Type], [TriggerType], [Status], [Points], [Timestamp]) 
VALUES (1, N'Registration Award', 'fa fa-user-check', 0, 1, 0, 250, @Current)
SET IDENTITY_INSERT [dbo].[Award] OFF
GO