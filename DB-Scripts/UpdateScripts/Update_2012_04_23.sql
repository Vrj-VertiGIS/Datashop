USE [$(geodatabase)]
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

/****** Object:  Table [$(gnsduser)].[gnsd_jobs]    Script Date: 02/20/2012 11:14:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [$(gnsduser)].[gnsd_jobs]

ADD [MapExtentCount] [int] NULL

GO

/****** Object:  Table [$(gnsduser)].[[GNSD_GRIDJOBVIEW]]    Script Date: 02/20/2012 11:14:22 ******/

ALTER VIEW [$(gnsduser)].[GNSD_GRIDJOBVIEW] AS SELECT
      GNSD_JOBS.JOBID,
      STATEDATE,
      CREATEDATE,
      GNSD_JOBS.USERID,
      GNSD_JOBS.REASONID,
      GNSD_REASONS.DESCRIPTION AS REASON,
      GNSD_JOBS.JOBOUTPUT,
      GNSD_JOBS.DEFINITION,
      GNSD_JOBS.CUSTOM1,
      GNSD_JOBS.CUSTOM2,
      GNSD_JOBS.CUSTOM3,
      GNSD_JOBS.STEP,
      GNSD_JOBS.STATUS,
      GNSD_JOBS.NEEDSPROCESSING,
      GNSD_JOBS.ISACTIVE,
      GNSD_JOBS.PROCESSID,
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
	  GNSD_JOBS.MAXEXTENTCOUNT,
      GNSD_SURROGATE_JOBS.SURROGATEUSERID,
      GNSD_SURROGATE_JOBS.REQUESTDATE,
      GNSD_SURROGATE_JOBS.REQUESTTYPE,
      GNSD_SURROGATE_JOBS.STOPAFTERPROCESS,
      GNSD_USERS.FIRSTNAME,
      GNSD_USERS.LASTNAME,
      GNSD_USERS.EMAIL
      FROM
      ((GNSD_JOBS left outer join
      (SELECT
      GNSD_JOBS_LOG.JOBID AS ID,
      MAX(TIMESTAMP) AS STATEDATE,
      MIN(TIMESTAMP) AS CREATEDATE
      FROM
      GNSD_JOBS_LOG
      GROUP BY
      JOBID) AS LOGS on GNSD_JOBS.JOBID = LOGS.ID)
        left outer join GNSD_USERS on GNSD_JOBS.USERID = GNSD_USERS.USERID) 
        left outer join GNSD_REASONS on GNSD_JOBS.REASONID = GNSD_REASONS.REASONID
        left outer join GNSD_SURROGATE_JOBS on GNSD_JOBS.JOBID = GNSD_SURROGATE_JOBS.JOBID
GO


/****** Object:  View [[$(gnsduser)].[MyRepresentativeJobs]    Script Date: 04/20/2012 16:14:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [$(gnsduser).[MyRepresentativeJobs]
AS
SELECT     gnsd_jobs.JobID, gnsd_jobGuid.JobGuid, gnsd_surrogate_jobs.SurrogateUserId AS RepresentativeUserId, 
                      gnsd_jobs.UserID AS RepresentedUserId, gnsd_jobs.DownloadCount, Logs.CreatedDate, 
                      gnsd_users.Firstname AS RepresentativeUserFirstname, gnsd_users.Lastname AS RepresentativeUserLastname, 
                      gnsd_users.Company AS RepresentativeUserCompany, gnsd_users.Email AS RepresentativeUserEmail, 
                      gnsd_users_1.Firstname AS RepresentedUserFirstname, gnsd_users_1.Lastname AS RepresentedUserLastname, 
                      gnsd_users_1.Company AS RepresentedUserCompany, gnsd_users_1.Email AS RepresentedUserEmail, gnsd_jobs.IsArchived
FROM         gnsd_jobGuid RIGHT OUTER JOIN
                      gnsd_jobs INNER JOIN
                      gnsd_users AS gnsd_users_1 ON gnsd_jobs.UserID = gnsd_users_1.UserID ON 
                      gnsd_jobGuid.JobId = gnsd_jobs.JobID LEFT OUTER JOIN
                      gnsd_users INNER JOIN
                      gnsd_surrogate_jobs ON gnsd_users.UserID = gnsd_surrogate_jobs.SurrogateUserId ON 
                      gnsd_jobs.JobID = gnsd_surrogate_jobs.JobId LEFT OUTER JOIN
                          (SELECT     JobID AS JobId, MIN(Timestamp) AS CreatedDate
                            FROM          gnsd_jobs_log
                            GROUP BY JobID) AS Logs ON gnsd_jobs.JobID = Logs.JobId

GO