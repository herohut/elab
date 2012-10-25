
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/23/2012 13:01:33
-- Generated from EDMX file: D:\HERO\elab-git\Eking.News\Eking.News.AdminSoftware\Model2.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WebCrawler];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_EntryEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RawEntries] DROP CONSTRAINT [FK_EntryEntry];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[RawEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RawEntries];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'RawEntries'
CREATE TABLE [dbo].[RawEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Url] nvarchar(max)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [Parent_Id] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'RawEntries'
ALTER TABLE [dbo].[RawEntries]
ADD CONSTRAINT [PK_RawEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Parent_Id] in table 'RawEntries'
ALTER TABLE [dbo].[RawEntries]
ADD CONSTRAINT [FK_RawEntryRawEntry]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[RawEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RawEntryRawEntry'
CREATE INDEX [IX_FK_RawEntryRawEntry]
ON [dbo].[RawEntries]
    ([Parent_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------