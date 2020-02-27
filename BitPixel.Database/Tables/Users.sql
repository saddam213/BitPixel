CREATE TABLE [dbo].[Users] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [Email]             NVARCHAR (128)  NOT NULL,
    [EmailConfirmed]    BIT             NOT NULL,
    [PasswordHash]      NVARCHAR (256)  NOT NULL,
    [SecurityStamp]     NVARCHAR (50)   NOT NULL,
    [LockoutEndDateUtc] DATETIME        NULL,
    [LockoutEnabled]    BIT             NOT NULL,
    [AccessFailedCount] INT             NOT NULL,
    [UserName]          NVARCHAR (50)   NOT NULL,
    [Points]            INT             NOT NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[Users]([UserName] ASC);

