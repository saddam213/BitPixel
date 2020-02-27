CREATE TABLE [dbo].[Prize] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [GameId]      INT            NOT NULL,
    [UserId]      INT            NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (128) NOT NULL,
    [Type]        TINYINT        NOT NULL,
    [Status]      TINYINT        NOT NULL,
    [X]           INT            NOT NULL,
    [Y]           INT            NOT NULL,
    [Points]      INT            NOT NULL,
    [IsClaimed]   BIT            NOT NULL,
    [Data]        NVARCHAR (256) NULL,
    [Data2]       NVARCHAR (256) NULL,
    [Data3]       NVARCHAR (256) NULL,
    [Data4]       NVARCHAR (256) NULL,
    [Data5]       NVARCHAR (256) NULL,
    [ClaimTime]   DATETIME2 (7)  NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Prize] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE NONCLUSTERED INDEX [IX_UserAudit]
    ON [dbo].[Prize]([UserId] ASC, [IsClaimed] ASC)
    INCLUDE([Points]);
GO

CREATE NONCLUSTERED INDEX [IX_PrizeClaimed]
    ON [dbo].[Prize]([GameId] ASC, [X] ASC, [Y] ASC, [IsClaimed] ASC);
GO