CREATE TABLE [dbo].[PaymentUserMethod] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [UserId]          INT             NOT NULL,
    [PaymentMethodId] INT             NOT NULL,
    [Status]          TINYINT         NOT NULL,
    [Data]            NVARCHAR (256)  NULL,
    [Data2]           NVARCHAR (256)  NULL,
    [Data3]           NVARCHAR (256)  NULL,
    [Data4]           NVARCHAR (256)  NULL,
    [Data5]           NVARCHAR (256)  NULL,
    [Updated]         DATETIME2 (7)   NOT NULL,
    [Timestamp]       DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_Deposit] PRIMARY KEY CLUSTERED ([Id] ASC)
);

