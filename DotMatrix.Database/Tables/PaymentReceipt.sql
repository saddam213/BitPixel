CREATE TABLE [dbo].[PaymentReceipt] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [UserId]              INT             NOT NULL,
    [PaymentMethodId]     INT             NOT NULL,
    [PaymentUserMethodId] INT             NOT NULL,
    [Status]              TINYINT         NOT NULL,
    [Description]         NVARCHAR (512)  NULL,
    [Amount]              DECIMAL(22, 8)  NOT NULL,
    [Rate]                DECIMAL(22, 8)  NOT NULL,
    [Points]              INT             NOT NULL,
    [Data]                NVARCHAR (256)  NULL,
    [Data2]               NVARCHAR (256)  NULL,
    [Data3]               NVARCHAR (256)  NULL,
    [Data4]               NVARCHAR (256)  NULL,
    [Data5]               NVARCHAR (256)  NULL,
    [Updated]             DATETIME2 (7)   NOT NULL,
    [Timestamp]           DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_PaymentReceipt] PRIMARY KEY CLUSTERED ([Id] ASC)
);

