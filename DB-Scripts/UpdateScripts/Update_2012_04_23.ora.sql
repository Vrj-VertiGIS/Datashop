﻿
SPOOL ./create_schema_ora.log

/****** Object:  Table [$(gnsduser)].[gnsd_placementoptions]    Script Date: 02/20/2012 11:14:22 ******/
CREATE TABLE &&1..GNSD_PLACEMENTOPTIONS 
(
  PLACEMENTOPTIONID NUMBER(8,0) NOT NULL ,
  TEXT VARCHAR2(255) ,
  CONSTRAINT GNSD_PLACEMENTOPTIONS_PK PRIMARY KEY 
  (
	PLACEMENTOPTIONID 
  )
  ENABLE 
)
/

CREATE SEQUENCE &&1..SEQ_GNSD_PLACEMENTOPTIONS_ID
	MINVALUE 1
	MAXVALUE 99999999
	START WITH 1
	INCREMENT BY 1
	NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_PLACEMENTOPTIONS_INSERT
	BEFORE INSERT ON &&1..GNSD_PLACEMENTOPTIONS
	FOR EACH ROW
	BEGIN
		IF :new.PLACEMENTOPTIONID IS NULL THEN
		SELECT &&1..SEQ_GNSD_PLACEMENTOPTIONS_ID.nextval INTO :new.PLACEMENTOPTIONID FROM DUAL;
		END IF;
	END;
/
COMMIT
/

ALTER TABLE &&1..GNSD_JOBS
AS

ADD COLUMN MAPEXTENTCOUNT INT

/
COMMIT
/

/****** Object:  View [$(gnsduser)].[GNSD_GRIDJOBVIEW]    Script Date: 02/16/2011 17:04:17 ******/
CREATE OR REPLACE FORCE VIEW &&1..GNSD_GRIDJOBVIEW ("JOBID", "STATEDATE", "CREATEDATE", "USERID", "REASONID", "REASON", "JOBOUTPUT", "DEFINITION", "CUSTOM1", "CUSTOM2", "CUSTOM3", "STEP", "STATUS", "NEEDSPROCESSING", "ISACTIVE", "PROCESSID", "PROCESSING_USERID", "PROCESSOR_CLASSID", "ISARCHIVED", "PERIODBEGINDATE", "PERIODENDDATE", "DESCRIPTION", "PARCELNUMBER", "MUNICIPALITY", "CENTERAREAX", "CENTERAREAY", "DOWNLOADCOUNT", "SURROGATEUSERID", "REQUESTDATE", "REQUESTTYPE", "STOPAFTERPROCESS", "FIRSTNAME", "LASTNAME", "EMAIL")
AS
  SELECT GNSD_JOBS.JOBID,
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
	GNSD_JOBS.MAPEXTENTCOUNT,
	GNSD_SURROGATE_JOBS.SURROGATEUSERID,
	GNSD_SURROGATE_JOBS.REQUESTDATE,
	GNSD_SURROGATE_JOBS.REQUESTTYPE,
	GNSD_SURROGATE_JOBS.STOPAFTERPROCESS,
	GNSD_USERS.FIRSTNAME,
	GNSD_USERS.LASTNAME,
	GNSD_USERS.EMAIL
  FROM &&1..GNSD_JOBS LEFT OUTER JOIN &&1..GNSD_USERS ON GNSD_JOBS.USERID = GNSD_USERS.USERID
	LEFT OUTER JOIN &&1..GNSD_REASONS ON GNSD_JOBS.REASONID = GNSD_REASONS.REASONID
	LEFT OUTER JOIN &&1..GNSD_SURROGATE_JOBS ON GNSD_JOBS.JOBID = GNSD_SURROGATE_JOBS.JOBID
	LEFT OUTER JOIN 
	  (SELECT GNSD_JOBS_LOG.JOBID AS ID,
		MAX(TIMESTAMP)            AS STATEDATE,
		MIN(TIMESTAMP)            AS CREATEDATE
	  FROM &&1..GNSD_JOBS_LOG
	  GROUP BY JOBID
	  ) logs ON GNSD_JOBS.JOBID  =+ LOGS.ID

/
COMMIT
/

/****** Object:  View [[$(gnsduser)].[MyRepresentativeJobs]    Script Date: 04/20/2012 16:14:47 ******/

CREATE OR REPLACE FORCE VIEW &&1..MyRepresentativeJobs ("JobGuid", "RepresentativeUserId", "RepresentedUserId", "DownloadCount", "CreatedDate", "RepresentativeUserFirstname", "RepresentativeUserLastname", "RepresentativeUserCompany", "RepresentativeUserEmail", "RepresentedUserFirstname", "RepresentedUserLastname", "RepresentedUserCompany", "RepresentedUserEmail", "IsArchived")
AS

SELECT     gnsd_jobs.JobID, gnsd_jobGuid.JobGuid, gnsd_surrogate_jobs.SurrogateUserId AS RepresentativeUserId, 
					  gnsd_jobs.UserID AS RepresentedUserId, gnsd_jobs.DownloadCount, Logs.CreatedDate, 
					  gnsd_users.Firstname AS RepresentativeUserFirstname, gnsd_users.Lastname AS RepresentativeUserLastname, 
					  gnsd_users.Company AS RepresentativeUserCompany, gnsd_users.Email AS RepresentativeUserEmail, 
					  gnsd_users_1.Firstname AS RepresentedUserFirstname, gnsd_users_1.Lastname AS RepresentedUserLastname, 
					  gnsd_users_1.Company AS RepresentedUserCompany, gnsd_users_1.Email AS RepresentedUserEmail, gnsd_jobs.IsArchived
FROM         &&1..gnsd_jobGuid RIGHT OUTER JOIN
					  &&1..gnsd_jobs INNER JOIN
					  &&1..gnsd_users AS gnsd_users_1 ON gnsd_jobs.UserID = gnsd_users_1.UserID ON 
					  gnsd_jobGuid.JobId = gnsd_jobs.JobID LEFT OUTER JOIN
					  &&1..gnsd_users INNER JOIN
					  &&1..gnsd_surrogate_jobs ON gnsd_users.UserID = gnsd_surrogate_jobs.SurrogateUserId ON 
					  gnsd_jobs.JobID = gnsd_surrogate_jobs.JobId LEFT OUTER JOIN
						  (SELECT     JobID AS JobId, MIN(Timestamp) AS CreatedDate
							FROM          gnsd_jobs_log
							GROUP BY JobID) AS Logs ON gnsd_jobs.JobID = Logs.JobId
/
COMMIT
/

SPOOL OFF
QUIT