﻿CREATE TABLE [dbo].[Team] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (128)  NOT NULL,
    CONSTRAINT [PK_Team] PRIMARY KEY CLUSTERED ([Id] ASC)
);
