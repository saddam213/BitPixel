CREATE TABLE [dbo].[UserInRoles] (
    [RoleId] INT NOT NULL,
    [UserId] INT NOT NULL,
    CONSTRAINT [PK_dbo.UserInRoles] PRIMARY KEY CLUSTERED ([RoleId] ASC, [UserId] ASC),
    CONSTRAINT [FK_dbo.UserInRoles_dbo.UserRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[UserRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserInRoles_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[UserInRoles]([RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserInRoles]([UserId] ASC);

