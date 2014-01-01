USE [master]
GO

CREATE LOGIN [adder] WITH PASSWORD=N'redda#2014', DEFAULT_DATABASE=[Advertize], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF

ALTER LOGIN [adder] ENABLE
GO


USE [Advertize]
GO

CREATE USER [adder] FOR LOGIN [adder] WITH DEFAULT_SCHEMA=[dbo]
GO

USE [Advertize]
GO

/****** Object:  Table [dbo].[Directory]    Script Date: 12/31/2013 11:52:55 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Directory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](2048) NOT NULL,
	[VersionStamp] [rowversion] NOT NULL,
 CONSTRAINT [PK_Directory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Advertisement]    Script Date: 12/31/2013 11:53:03 PM ******/

CREATE TABLE [dbo].[Advertisement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DirectoryId] [int] NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](2048) NOT NULL,
	[Url] [nvarchar](1024) NOT NULL,
	[VersionStamp] [rowversion] NOT NULL,
 CONSTRAINT [PK_Advertisement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Advertisement]  WITH CHECK ADD  CONSTRAINT [FK_Advertisement_Directory] FOREIGN KEY([DirectoryId])
REFERENCES [dbo].[Directory] ([Id])
GO

ALTER TABLE [dbo].[Advertisement] CHECK CONSTRAINT [FK_Advertisement_Directory]
GO

