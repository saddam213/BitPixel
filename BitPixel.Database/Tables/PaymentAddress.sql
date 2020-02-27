CREATE TABLE [dbo].[PaymentAddress] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [PaymentMethodId] INT             NOT NULL,
    [UserId]          INT             NULL,
    [Address]         NVARCHAR (256)  NOT NULL,
    [Updated]         DATETIME2 (7)     NOT NULL,
    [Timestamp]       DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_PaymentAddress] PRIMARY KEY CLUSTERED ([Id] ASC)
);

