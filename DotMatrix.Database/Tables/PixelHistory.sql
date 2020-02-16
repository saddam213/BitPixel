CREATE TABLE [dbo].[PixelHistory] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [PixelId]   INT             NOT NULL,
    [GameId]      INT           NOT NULL,
    [UserId]    INT             NOT NULL,
    [Type]      TINYINT         NOT NULL,
    [Color]     NVARCHAR(7)     NOT NULL,
    [Points]    INT             NOT NULL,
    [Timestamp] DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_PixelHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_UserAudit]
    ON [dbo].[PixelHistory]([UserId] ASC)
    INCLUDE([Points]);
GO
