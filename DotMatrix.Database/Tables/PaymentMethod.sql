CREATE TABLE [dbo].[PaymentMethod] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Type]          TINYINT         NOT NULL,
    [Status]        TINYINT         NOT NULL,
    [Name]          NVARCHAR (128)  NOT NULL,
    [Symbol]          NVARCHAR (10)  NOT NULL,
    [Description]   NVARCHAR (512)  NOT NULL,
    [Note]          NVARCHAR (256)  NOT NULL,
    [Rate]                DECIMAL(22, 8)  NOT NULL,
    [Data]                NVARCHAR (256)  NULL,
    [Data2]               NVARCHAR (256)  NULL,
    [Data3]               NVARCHAR (256)  NULL,
    [Data4]               NVARCHAR (256)  NULL,
    [Data5]               NVARCHAR (256)  NULL,
    [Updated]       DATETIME2 (7)   NOT NULL,
    [Timestamp]     DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_PaymentMethod] PRIMARY KEY CLUSTERED ([Id] ASC)
);

