USE [$(geodatabase)]
GO

ALTER TABLE [$(gnsduser)].[gnsd_plotdefinitions]
	ADD [LimitsTimePeriods] [nvarchar](255) NULL,
		[Limits] [nvarchar](255) NULL


GO
