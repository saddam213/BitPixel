CREATE TABLE [dbo].[Award] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Icon]        NVARCHAR (50)  NOT NULL,
    [Type]        SMALLINT       NOT NULL,
    [TriggerType] TINYINT        NOT NULL,
    [Status]      TINYINT        NOT NULL,
    [Points]      INT            NOT NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED ([Id] ASC)
);

