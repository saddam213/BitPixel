CREATE TABLE [dbo].[AwardHistory] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [AwardId]     INT           NOT NULL,
    [UserId]      INT           NOT NULL,
    [Version]     NVARCHAR(128) NOT NULL,
    [VersionData] NVARCHAR(50)  NULL,
    [Points]      INT           NOT NULL,
    [Timestamp]   DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_AwardHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AwardUserVersion]
    ON [dbo].[AwardHistory]([AwardId] ASC, [UserId] ASC, [Version] ASC);
GO