CREATE TABLE [dbo].[Click] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [GameId]      INT           NOT NULL,
    [UserId]     INT            NOT NULL,
    [Type]       TINYINT        NOT NULL,
    [X]          INT            NOT NULL,
    [Y]          INT            NOT NULL,
    [Timestamp] DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_Click] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

