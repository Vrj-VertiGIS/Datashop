USE [$(geodatabase)]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_biz_users]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_biz_users](
	[BIZUSERID] [bigint] IDENTITY(1,1) NOT NULL,
	[Password] [nvarchar](255) NULL,
	[PasswordSalt] [nvarchar](255) NULL,
	[Roles] [nvarchar](255) NULL,
	[Status] [int] NULL,
	[FailedLoginCount] [int] NULL,
	[BlockedUntil] [datetime] NULL,
        [PasswordResetId] [nvarchar](255) NULL, --GUID
	[PasswordResetIdValidity] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BIZUSERID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_users]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_users](
	[UserID] [bigint] IDENTITY(1,1) NOT NULL,
	[Adress] [nvarchar](255) NULL,
	[Firstname] [nvarchar](255) NULL,
	[Lastname] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[Street] [nvarchar](255) NULL,
	[Streetnr] [nvarchar](255) NULL,
	[Citycode] [nvarchar](255) NULL,
	[City] [nvarchar](255) NULL,
	[Company] [nvarchar](255) NULL,
	[Tel] [nvarchar](255) NULL,
	[Fax] [nvarchar](255) NULL,
	[BIZUSER] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_surrogate_jobs]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_surrogate_jobs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[UserId] [bigint] NULL,
	[SurrogateUserId] [bigint] NULL,
	[RequestDate] [datetime] NULL,
	[RequestType] [nvarchar](255) NULL,
	[StopAfterProcess] [bit] NULL,
 CONSTRAINT [PK_gnsd_surrogate_jobs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_reasons]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_reasons](
	[ReasonID] [bigint] IDENTITY(2,1) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[PeriodDateRequired] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReasonID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_plotdefinitions]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_plotdefinitions](
	[MediumCODE] [int] NOT NULL,
	[Template] [nvarchar](255) NOT NULL,
	[PlotHeightCM] [float] NULL,
	[PlotWidthCM] [float] NULL,
	[Description] [nvarchar](255) NULL,
	[Roles] [nvarchar](255) NULL,
	[LimitsTimePeriods] [nvarchar](255) NULL,
	[Limits] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[MediumCODE] ASC,
	[Template] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_plot_sections]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_plot_sections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[VisibleGroupLayers] [nvarchar](max) NULL,
 CONSTRAINT [PK_gnsd_plot_sections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_placementoptions]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_placementoptions](
	[PlacementOptionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_gnsd_placementoptions] PRIMARY KEY CLUSTERED 
(
	[PlacementOptionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_jobs_log]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_jobs_log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobID] [bigint] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Message] [nvarchar](255) NULL,
	[Status] [int] NULL,
	[Step] [int] NULL,
	[NeedsProcessing] [bit] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [$(gnsduser)].[gnsd_jobs]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_jobs](
	[JobID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[ReasonID] [bigint] NULL,
	[Definition] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[Step] [int] NULL,
	[NeedsProcessing] [bit] NULL,
	[IsActive] [bit] NULL,
	[JobOutput] [nvarchar](255) NULL,
	[ProcessID] [bigint] NULL,
    [MachineName] [nvarchar](255) NULL,
	[Processing_UserID] [bigint] NULL,
	[Processor_ClassID] [nvarchar](255) NULL,
	[Custom1] [nvarchar](max) NULL,
	[Custom2] [nvarchar](max) NULL,
	[Custom3] [nvarchar](max) NULL,
    [Custom4] [nvarchar](max) NULL,
	[Custom5] [nvarchar](max) NULL,
	[Custom6] [nvarchar](max) NULL,
	[Custom7] [nvarchar](max) NULL,
	[Custom8] [nvarchar](max) NULL,
	[Custom9] [nvarchar](max) NULL,
	[Custom10] [nvarchar](max) NULL,
	[IsArchived] [char](1) NULL,
	[PeriodBeginDate] [datetime] NULL,
	[PeriodEndDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[ParcelNumber] [nvarchar](255) NULL,
	[Municipality] [nvarchar](max) NULL,
	[CenterAreaX] [float] NULL,
	[CenterAreaY] [float] NULL,
	[DownloadCount] [int] NULL,
	[MapExtentCount] [int] NULL,
	[GeoAttachmentsEnabled] [bit] NULL,
	[StateDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[DxfExport] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [GNSD_JOBS_CLASSID_ARCHIVED] ON [$(gnsduser)].[gnsd_jobs]
(
	[Processor_ClassID] ASC,
	[IsArchived] ASC
)
GO

CREATE TRIGGER [$(gnsduser)].TRG_GNSD_JOB_LOG_STATEDATE 
   ON  [$(gnsduser)].GNSD_JOBS_LOG 
   AFTER INSERT
AS 
	BEGIN
		BEGIN
		  UPDATE [$(gnsduser)].GNSD_JOBS
		  SET STATEDATE = inserted.TIMESTAMP
		  FROM [$(gnsduser)].GNSD_JOBS JOIN inserted
		  ON GNSD_JOBS.JobID = inserted.JOBID;
		END ;
	END
GO


SET ANSI_PADDING OFF
GO
/****** Object:  Table [$(gnsduser)].[gnsd_jobGuid]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [$(gnsduser)].[gnsd_jobGuid](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobGuid] [nvarchar](255) NULL,
	[JobId] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [$(gnsduser)].[GNSD_GRIDJOBVIEW]    Script Date: 02/20/2012 11:14:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [$(gnsduser)].[GNSD_GRIDJOBVIEW] AS SELECT
      GNSD_JOBS.JOBID,
      GNSD_JOBS.STATEDATE,
      GNSD_JOBS.CREATEDATE,
      GNSD_JOBS.USERID,
      GNSD_JOBS.REASONID,
      GNSD_REASONS.DESCRIPTION AS REASON,
      GNSD_JOBS.JOBOUTPUT,
      GNSD_JOBS.DEFINITION,
      GNSD_JOBS.CUSTOM1,
      GNSD_JOBS.CUSTOM2,
      GNSD_JOBS.CUSTOM3,
      GNSD_JOBS.CUSTOM4,
      GNSD_JOBS.CUSTOM5,
      GNSD_JOBS.CUSTOM6,
      GNSD_JOBS.CUSTOM7,
      GNSD_JOBS.CUSTOM8,
      GNSD_JOBS.CUSTOM9,
      GNSD_JOBS.CUSTOM10,
      GNSD_JOBS.STEP,
      GNSD_JOBS.STATUS,
      GNSD_JOBS.NEEDSPROCESSING,
      GNSD_JOBS.ISACTIVE,
      GNSD_JOBS.PROCESSID,
      GNSD_JOBS.MACHINENAME,
      GNSD_JOBS.PROCESSING_USERID,
      GNSD_JOBS.PROCESSOR_CLASSID,
      GNSD_JOBS.ISARCHIVED,
      GNSD_JOBS.PERIODBEGINDATE, 
      GNSD_JOBS.PERIODENDDATE,
      GNSD_JOBS.DESCRIPTION,
      GNSD_JOBS.PARCELNUMBER,
      GNSD_JOBS.MUNICIPALITY,
      GNSD_JOBS.CENTERAREAX,
      GNSD_JOBS.CENTERAREAY,
      GNSD_JOBS.DOWNLOADCOUNT,
	  GNSD_JOBS.MAPEXTENTCOUNT,
	  GNSD_JOBS.GEOATTACHMENTSENABLED,
	  GNSD_JOBS.DxfExport,
      GNSD_SURROGATE_JOBS.SURROGATEUSERID,
      GNSD_SURROGATE_JOBS.REQUESTDATE,
      GNSD_SURROGATE_JOBS.REQUESTTYPE,
      GNSD_SURROGATE_JOBS.STOPAFTERPROCESS,
      GNSD_USERS.FIRSTNAME,
      GNSD_USERS.LASTNAME,
      GNSD_USERS.EMAIL     
      FROM
      GNSD_JOBS left outer join GNSD_USERS on GNSD_JOBS.USERID = GNSD_USERS.USERID
        left outer join GNSD_REASONS on GNSD_JOBS.REASONID = GNSD_REASONS.REASONID
        left outer join GNSD_SURROGATE_JOBS on GNSD_JOBS.JOBID = GNSD_SURROGATE_JOBS.JOBID
GO

/****** Object:  View [[$(gnsduser)].[MyRepresentativeJobs]    Script Date: 04/20/2012 16:14:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [$(gnsduser)].[MyRepresentativeJobs]
AS
SELECT        gnsd_jobs.JobID, gnsd_jobGuid.JobGuid, gnsd_surrogate_jobs.SurrogateUserId AS RepresentativeUserId, 
              gnsd_jobs.UserID AS RepresentedUserId, gnsd_jobs.DownloadCount, Logs.CreatedDate, 
              gnsd_users.Firstname AS RepresentativeUserFirstname, gnsd_users.Lastname AS RepresentativeUserLastname, 
              gnsd_users.Company AS RepresentativeUserCompany, gnsd_users.Email AS RepresentativeUserEmail, 
              gnsd_users_1.Firstname AS RepresentedUserFirstname, gnsd_users_1.Lastname AS RepresentedUserLastname, 			 
              gnsd_users_1.Company AS RepresentedUserCompany, gnsd_users_1.Email AS RepresentedUserEmail, gnsd_jobs.IsArchived, 
              gnsd_jobs.Custom1, gnsd_jobs.Custom2, gnsd_jobs.Custom3, gnsd_jobs.Custom4, gnsd_jobs.Custom5, 
              gnsd_jobs.Custom6, gnsd_jobs.Custom7, gnsd_jobs.Custom8, gnsd_jobs.Custom9, gnsd_jobs.Custom10, 
              gnsd_jobs.ParcelNumber, gnsd_jobs.ReasonID, gnsd_reasons.Description AS Reason,
              gnsd_jobs.Status, gnsd_jobs.Step
FROM          
              gnsd_reasons INNER JOIN
              gnsd_jobs INNER JOIN
              gnsd_users AS gnsd_users_1 ON gnsd_jobs.UserID = gnsd_users_1.UserID ON 
              gnsd_reasons.ReasonID = gnsd_jobs.ReasonID LEFT OUTER JOIN
              gnsd_jobGuid ON gnsd_jobs.JobID = gnsd_jobGuid.JobId LEFT OUTER JOIN
              gnsd_users INNER JOIN
              gnsd_surrogate_jobs ON gnsd_users.UserID = gnsd_surrogate_jobs.SurrogateUserId ON 
              gnsd_jobs.JobID = gnsd_surrogate_jobs.JobId LEFT OUTER JOIN
              (SELECT JobID AS JobId, MIN(Timestamp) AS CreatedDate
              FROM gnsd_jobs_log
              GROUP BY JobID) AS Logs ON gnsd_jobs.JobID = Logs.JobId

GO
