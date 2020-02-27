CREATE TABLE [dbo].[Award] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (256) NULL,
    [Icon]        NVARCHAR (50)  NOT NULL,
    [Type]        SMALLINT       NOT NULL,
    [Level]       TINYINT        NOT NULL,
    [TriggerType] TINYINT        NOT NULL,
    [ClickType]   TINYINT        NOT NULL,
    [Status]      TINYINT        NOT NULL,
    [Points]      INT            NOT NULL,
    [Rank]        INT            NOT NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED ([Id] ASC)
);

