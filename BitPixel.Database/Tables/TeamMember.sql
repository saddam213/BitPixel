CREATE TABLE [dbo].[TeamMember] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [TeamId]      INT            NOT NULL,
    [UserId]      INT            NOT NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_TeamMember] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_TeamUser]
    ON [dbo].[TeamMember]([TeamId] ASC, [UserId] ASC);
GO