
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/26/2012 15:40:14
-- Generated from EDMX file: D:\EcoCounterAzure\MvcWebRole1\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ecocounter];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[COMPANIES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[COMPANIES];
GO
IF OBJECT_ID(N'[dbo].[GROUPS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GROUPS];
GO
IF OBJECT_ID(N'[dbo].[ROLES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ROLES];
GO
IF OBJECT_ID(N'[dbo].[USERS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[USERS];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'COMPANIES'
CREATE TABLE [dbo].[COMPANIES] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NAME] nvarchar(30)  NULL
);
GO

-- Creating table 'GROUPS'
CREATE TABLE [dbo].[GROUPS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NAME] nvarchar(30)  NULL,
    [COMPANY_ID] int  NOT NULL
);
GO

-- Creating table 'ROLES'
CREATE TABLE [dbo].[ROLES] (
    [ID] int  NOT NULL,
    [NAME] varchar(30)  NULL,
    [NAME_JA] nvarchar(30)  NULL
);
GO

-- Creating table 'USERS'
CREATE TABLE [dbo].[USERS] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [GROUP_ID] int  NOT NULL,
    [USERID] varchar(30)  NOT NULL,
    [NAME] nvarchar(30)  NOT NULL,
    [PASSWORD] varchar(8000)  NOT NULL,
    [LOGIN_KEY] varchar(50)  NULL,
    [ROLE_ID] int  NOT NULL,
    [APPLY_DATE] datetime  NULL,
    [DEL_FLAG] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'COMPANIES'
ALTER TABLE [dbo].[COMPANIES]
ADD CONSTRAINT [PK_COMPANIES]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'GROUPS'
ALTER TABLE [dbo].[GROUPS]
ADD CONSTRAINT [PK_GROUPS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ROLES'
ALTER TABLE [dbo].[ROLES]
ADD CONSTRAINT [PK_ROLES]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'USERS'
ALTER TABLE [dbo].[USERS]
ADD CONSTRAINT [PK_USERS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------