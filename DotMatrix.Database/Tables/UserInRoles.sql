CREATE TABLE [dbo].[UserInRoles] (
    [RoleId] INT NOT NULL,
    [UserId] INT NOT NULL,
    CONSTRAINT [PK_dbo.UserInRoles] PRIMARY KEY CLUSTERED ([RoleId] ASC, [UserId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[UserInRoles]([RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserInRoles]([UserId] ASC);

