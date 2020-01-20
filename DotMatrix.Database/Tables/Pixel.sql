CREATE TABLE [dbo].[Pixel] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     INT            NOT NULL,
    [X]          INT            NOT NULL,
    [Y]          INT            NOT NULL,
    [Type]       TINYINT        NOT NULL,
    [Color]      NVARCHAR(7)    NOT NULL,
    [Points]     INT            NOT NULL,
    [LastUpdate] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Pixel] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UIX_XY]
    ON [dbo].[Pixel]([X] ASC, [Y] ASC);
GO