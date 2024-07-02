CREATE DATABASE [TeamManager]
GO

USE [TeamManager]
GO

CREATE TABLE dbo.Member (
	[Id]               [int] IDENTITY(1,1) NOT NULL,
	[Name]             [varchar](100) NOT NULL,
	[SalaryPerYear]    [decimal](18, 2) NOT NULL,
	[Type]             [int] NULL,
	[ContractDuration] [int] NULL,
	[Role]             [varchar](100) NULL,
	[Tags]             [varchar](200) NULL,
	[CountryName]      [varchar](100) NULL,
	[CurrencyCode]     [varchar](10) NULL,
	[CurrencySymbol]   [varchar](10) NULL,
	[CurrencyName]     [varchar](100) NULL,
    CONSTRAINT [PK_Member] PRIMARY KEY ([Id])
)
GO

CREATE TABLE dbo.[User] (
	[Id]               [int] IDENTITY(1,1) NOT NULL,
	[UserName]         [varchar](100) NOT NULL,
    [Password]         [varchar](150) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
)
GO

CREATE UNIQUE INDEX IDX_UserName on dbo.[User] (UserName)
GO