CREATE TABLE [dbo].[GamePrize] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [GameId]      INT            NOT NULL,
    [UserId]      INT            NULL,
    [TeamId]      INT            NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (128) NOT NULL,
    [Type]        TINYINT        NOT NULL,
    [Points]      INT            NOT NULL,
    [Rank]        INT            NOT NULL,
    [Data]        NVARCHAR (256) NULL,
    [Data2]       NVARCHAR (256) NULL,
    [Data3]       NVARCHAR (256) NULL,
    [Data4]       NVARCHAR (256) NULL,
    [Data5]       NVARCHAR (256) NULL,
    [ClaimTime]   DATETIME2 (7)  NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_GamePrize] PRIMARY KEY CLUSTERED ([Id] ASC)
);