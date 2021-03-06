
SPOOL ./create_schema_ora.log

ALTER TABLE &&1..GNSD_JOBS 
ADD GEOATTACHMENTSENABLED INT

/
COMMIT
/

CREATE OR REPLACE FORCE VIEW &&1..GNSD_GRIDJOBVIEW ("JOBID", "STATEDATE", "CREATEDATE", "USERID", "REASONID", "REASON", "JOBOUTPUT", "DEFINITION", "CUSTOM1", "CUSTOM2", "CUSTOM3", "STEP", "STATUS", "NEEDSPROCESSING", "ISACTIVE", "PROCESSID", "PROCESSING_USERID", "PROCESSOR_CLASSID", "ISARCHIVED", "PERIODBEGINDATE", "PERIODENDDATE", "DESCRIPTION", "PARCELNUMBER", "MUNICIPALITY", "CENTERAREAX", "CENTERAREAY", "DOWNLOADCOUNT", "MAPEXTENTCOUNT", "GEOATTACHMENTSENABLED", "SURROGATEUSERID", "REQUESTDATE", "REQUESTTYPE", "STOPAFTERPROCESS", "FIRSTNAME", "LASTNAME", "EMAIL")
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
	GNSD_JOBS.GEOATTACHMENTSENABLED,
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


SPOOL OFF
QUIT