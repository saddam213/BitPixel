CREATE TABLE [dbo].[Game] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (265) NOT NULL,
    [Type]        TINYINT        NOT NULL,
    [Status]      TINYINT        NOT NULL,
    [Platform]    TINYINT        NOT NULL,
    [EndType]     TINYINT        NOT NULL,
    [EndTime]     DATETIME2 (7)  NULL,
    [Width]       INT            NOT NULL,
    [Height]      INT            NOT NULL,
    [ClicksPerSecond] INT        NOT NULL,
    [Rank]        INT            NOT NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    [LastUpdated] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED ([Id] ASC)
);

