CREATE TABLE [dbo].[Click] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]     INT            NOT NULL,
    [Type]       TINYINT        NOT NULL,
    [X]          INT            NOT NULL,
    [Y]          INT            NOT NULL,
    [Timestamp] DATETIME2 (1)   NOT NULL,
    CONSTRAINT [PK_Click] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_UserTimestamp]
    ON [dbo].[Click]([UserId] ASC, [Timestamp] ASC);
GO