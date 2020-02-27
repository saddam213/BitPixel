CREATE TABLE [dbo].[Team] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [GameId]      INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (265) NULL,
    [Icon]        NVARCHAR (50)  NULL,
    [Color]       NVARCHAR(7)    NOT NULL,
    [Rank]        INT            NOT NULL,
    [Result]      INT            NOT NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Team] PRIMARY KEY CLUSTERED ([Id] ASC)
);