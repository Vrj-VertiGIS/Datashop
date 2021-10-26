USE [$(geodatabase)]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [$(gnsduser)].[GNSD_BIZ_USERS]
	ADD 
	[FailedLoginCount] [int] NULL,
	[BlockedUntil] [datetime] NULL
GO