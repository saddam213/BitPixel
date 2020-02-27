CREATE TABLE [dbo].[Pixel] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [GameId]      INT           NOT NULL,
    [UserId]     INT            NOT NULL,
    [TeamId]    INT             NULL,
    [X]          INT            NOT NULL,
    [Y]          INT            NOT NULL,
    [Type]       TINYINT        NOT NULL,
    [Color]      NVARCHAR(7)    NOT NULL,
    [Points]     INT            NOT NULL,
    [LastUpdate] DATETIME2 (7)  NOT NULL,
    [LastUpdated] ROWVERSION NOT NULL,
    CONSTRAINT [PK_Pixel] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UIX_XY]
    ON [dbo].[Pixel]([GameId] ASC, [X] ASC, [Y] ASC)
    INCLUDE([UserId], [Type], [Color], [Points]);
GO