USE [DataMaskingDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Address]
(
	[AddressID] [int] IDENTITY(1,1) NOT NULL,
	[AddressLine1] [nvarchar](60) NOT NULL,
	[AddressLine2] [nvarchar](60) NULL,
	[CityID] [int] NULL,
	[CountryID] [int] NOT NULL,
	[PostalCode] [nvarchar](15) NOT NULL,
	[MapDataJson] [nvarchar](max) NULL,
	[MapDataXml] [xml] NULL,
	[ModifiedDate] [datetime2] NOT NULL DEFAULT getutcdate(),
	PRIMARY KEY (AddressID)
);
GO
