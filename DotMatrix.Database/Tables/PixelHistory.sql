CREATE TABLE [dbo].[PixelHistory] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [PixelId]   INT             NOT NULL,
    [UserId]    INT             NOT NULL,
    [Type]      TINYINT         NOT NULL,
    [Color]     NVARCHAR(7)     NOT NULL,
    [Points]    INT             NOT NULL,
    [Timestamp] DATETIME2 (1)   NOT NULL,
    CONSTRAINT [PK_PixelHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_UserTimestamp]
    ON [dbo].[PixelHistory]([UserId] ASC, [Timestamp] ASC);
GO