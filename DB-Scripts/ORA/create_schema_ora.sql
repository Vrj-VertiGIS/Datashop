SPOOL ./create_schema_ora.log

-- USER SQL
CREATE USER &&1 IDENTIFIED BY &&2
DEFAULT TABLESPACE &&3;


-- QUOTAS
ALTER USER &&1 QUOTA UNLIMITED ON &&3;


-- ROLES
GRANT "CONNECT" TO &&1;

-- SYSTEM PRIVILEGES TO CREATE TABLE IN OWN SCHEMA
GRANT CREATE TABLE TO &&1;


CREATE TABLE &&1..GNSD_BIZ_USERS 
(
    BIZUSERID NUMBER(6,0) NOT NULL ,
    PASSWORD VARCHAR2(50) ,
    PASSWORDSALT VARCHAR2(255),
    ROLES VARCHAR2(255) ,
    STATUS INT ,
    FAILEDLOGINCOUNT INT , 
    BLOCKEDUNTIL DATE NULL ,
    PASSWORDRESETID VARCHAR2(36 BYTE) NULL , --GUID
    PASSWORDRESETIDVALIDITY DATE NULL ,
    CONSTRAINT GNSD_BIZ_USERS_PK PRIMARY KEY 
  (
    BIZUSERID 
  )
    ENABLE 
)
/

CREATE SEQUENCE &&1..SEQ_GNSD_BIZ_USERS_ID
    MINVALUE 1
    MAXVALUE 999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_BIZ_USERS_INSERT
    BEFORE INSERT ON &&1..GNSD_BIZ_USERS
    FOR EACH ROW
    BEGIN
        IF :new.BIZUSERID IS NULL THEN
        SELECT &&1..SEQ_GNSD_BIZ_USERS_ID.nextval INTO :new.BIZUSERID FROM DUAL;
        END IF;
    END;
/ 
COMMIT
/


CREATE TABLE &&1..GNSD_USERS 
(
    USERID NUMBER(6,0) NOT NULL ,
    ADRESS VARCHAR2(255 BYTE) ,
    FIRSTNAME VARCHAR2(255 BYTE) ,
    LASTNAME VARCHAR2(255 BYTE) ,
    EMAIL VARCHAR2(255 BYTE) ,
    STREET VARCHAR2(255 BYTE) ,
    STREETNR VARCHAR2(255 BYTE) ,
    CITYCODE VARCHAR2(50 BYTE) ,
    CITY VARCHAR2(255 BYTE) ,
    COMPANY VARCHAR2(255 BYTE) ,
    TEL VARCHAR2(255 BYTE) ,
    FAX VARCHAR2(255 BYTE) ,
    BIZUSER NUMBER ,
    CONSTRAINT GNSD_USERS_PK PRIMARY KEY 
  (
    USERID 
  )
    ENABLE 
) 
/

CREATE SEQUENCE &&1..SEQ_GNSD_USERS_ID
    MINVALUE 1
    MAXVALUE 999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_USERS_INSERT
    BEFORE INSERT ON &&1..GNSD_USERS
    FOR EACH ROW
    BEGIN
        IF :new.USERID IS NULL THEN
        SELECT &&1..SEQ_GNSD_USERS_ID.nextval INTO :new.USERID FROM DUAL;
        END IF;
    END;
/
COMMIT
/

CREATE TABLE &&1..GNSD_REASONS 
(
    REASONID NUMBER(6,0) NOT NULL ,
    DESCRIPTION VARCHAR2(255) ,
    PERIODDATEREQUIRED NUMERIC(1, 0) ,
    CONSTRAINT GNSD_REASONS_PK PRIMARY KEY 
  (
    REASONID 
  )
    ENABLE 
)
/

CREATE SEQUENCE &&1..SEQ_GNSD_REASONS_ID
    MINVALUE 1
    MAXVALUE 999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_REASONS_INSERT
    BEFORE INSERT ON &&1..GNSD_REASONS
    FOR EACH ROW
    BEGIN
        IF :new.REASONID IS NULL THEN
        SELECT &&1..SEQ_GNSD_REASONS_ID.nextval INTO :new.REASONID FROM DUAL;
        END IF;
    END;
/
COMMIT
/

CREATE TABLE &&1..GNSD_PLOTDEFINITIONS 
(
    MEDIUMCODE INT NOT NULL ,
    TEMPLATE VARCHAR2(255) NOT NULL ,
    PLOTHEIGHTCM NUMERIC(10, 3) ,
    PLOTWIDTHCM NUMERIC(10, 3) ,
    DESCRIPTION VARCHAR2(255) ,
    ROLES VARCHAR2(255) ,
    LIMITSTIMEPERIODS VARCHAR2(255),
    LIMITS VARCHAR2(255),
    CONSTRAINT GNSD_PLOTDEFINITIONS_PK PRIMARY KEY 
  (
    MEDIUMCODE 
  , TEMPLATE 
  )
    ENABLE 
)
/

CREATE TABLE &&1..GNSD_JOBS_LOG 
(
    ID NUMBER(12,0) NOT NULL,
    JOBID NUMBER NOT NULL ,
    TIMESTAMP DATE NOT NULL ,
    MESSAGE VARCHAR2(255) ,
    STATUS INT ,
    STEP INT ,
    NEEDSPROCESSING INT ,
    ISACTIVE INT ,
    CONSTRAINT GNSD_JOBS_LOG_PK PRIMARY KEY 
  (
    ID 
  )
    ENABLE 
)
/

CREATE SEQUENCE &&1..SEQ_GNSD_JOBS_LOG_ID
    MINVALUE 1
    MAXVALUE 999999999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_JOBS_LOG_INSERT
    BEFORE INSERT ON &&1..GNSD_JOBS_LOG
    FOR EACH ROW
    BEGIN
        IF :new.ID IS NULL THEN
        SELECT &&1..SEQ_GNSD_JOBS_LOG_ID.nextval INTO :new.ID FROM DUAL;
        END IF;
    END;
/
COMMIT
/

CREATE TABLE &&1..GNSD_JOBS 
(
    JOBID NUMBER(12,0) NOT NULL ,
    USERID NUMBER ,
    REASONID NUMBER ,
    DEFINITION NCLOB,
    STATUS INT ,
    STEP INT ,
    NEEDSPROCESSING INT ,
    ISACTIVE INT ,
    JOBOUTPUT VARCHAR2(255) ,
    PROCESSID INT ,
    MACHINENAME VARCHAR2(255) ,
    PROCESSING_USERID INT ,
    PROCESSOR_CLASSID VARCHAR2(255) ,
    CUSTOM1 VARCHAR2(2000) ,
    CUSTOM2 VARCHAR2(2000) ,
    CUSTOM3 VARCHAR2(2000) ,
    CUSTOM4 VARCHAR2(2000) ,
    CUSTOM5 VARCHAR2(2000) ,
    CUSTOM6 VARCHAR2(2000) ,
    CUSTOM7 VARCHAR2(2000) ,
    CUSTOM8 VARCHAR2(2000) ,
    CUSTOM9 VARCHAR2(2000) ,
    CUSTOM10 VARCHAR2(2000) ,
    ISARCHIVED CHAR(1) ,
    PERIODBEGINDATE DATE ,
    PERIODENDDATE DATE ,
    DESCRIPTION VARCHAR2(2000) ,
    PARCELNUMBER VARCHAR2(255) ,
    MUNICIPALITY VARCHAR2(2000) ,
    CENTERAREAX FLOAT ,
    CENTERAREAY FLOAT ,
    DOWNLOADCOUNT INT ,
    MAPEXTENTCOUNT INT ,
    GEOATTACHMENTSENABLED INT ,
    STATEDATE DATE NULL ,
    CREATEDATE DATE NULL ,
    DXFEXPORT INT ,
    CONSTRAINT GNSD_JOBS_PK PRIMARY KEY 
  (
    JOBID 
  )
    ENABLE 
)
/

-- make queries on jobs faster. Columns PROCESSOR_CLASSID and ISARCHIVED are used 
CREATE INDEX &&1..GNSD_JOBS_CLASSID_ARCHIVED ON &&1..GNSD_JOBS ("PROCESSOR_CLASSID", "ISARCHIVED");
/

CREATE SEQUENCE &&1..SEQ_GNSD_JOBS_ID
    MINVALUE 1
    MAXVALUE 999999999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_JOBS_INSERT
    BEFORE INSERT ON &&1..GNSD_JOBS
    FOR EACH ROW
    BEGIN
        IF :new.JOBID IS NULL THEN
        SELECT &&1..SEQ_GNSD_JOBS_ID.nextval INTO :new.JOBID FROM DUAL;
        END IF;
    END;
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_JOB_LOG_STATEDATE
    AFTER INSERT ON &&1..GNSD_JOBS_LOG
    FOR EACH ROW
    BEGIN
      UPDATE &&1..GNSD_JOBS
      SET STATEDATE = :new.TIMESTAMP
      WHERE JOBID = :new.JOBID;
    END TRG_GNSD_JOB_LOG_STATEDATE;

/
COMMIT
/

CREATE TABLE &&1..GNSD_JOBGUID 
(
    ID NUMBER(12,0) NOT NULL ,
    JOBGUID VARCHAR2(36 BYTE) ,
    JOBID NUMBER ,
    CONSTRAINT GNSD_JOBGUID_PK PRIMARY KEY 
  (
    ID 
  )
    ENABLE 
) 
/

CREATE SEQUENCE &&1..SEQ_GNSD_JOBGUID_ID
    MINVALUE 1
    MAXVALUE 999999999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_JOBGUID
    BEFORE INSERT ON &&1..GNSD_JOBGUID
    FOR EACH ROW
    BEGIN
        IF :new.ID IS NULL THEN
        SELECT &&1..SEQ_GNSD_JOBGUID_ID.nextval INTO :new.ID FROM DUAL;
        END IF;
    END;
/
COMMIT
/

CREATE TABLE &&1..GNSD_PLOT_SECTIONS 
(
    ID NUMBER(5,0) NOT NULL ,
    NAME VARCHAR2(255) ,
    DESCRIPTION VARCHAR2(2000) ,
    VISIBLEGROUPLAYERS VARCHAR2(2000) ,
    CONSTRAINT GNSD_PLOT_SECTIONS_PK PRIMARY KEY 
  (
    ID 
  )
    ENABLE 
)
/

CREATE SEQUENCE &&1..SEQ_GNSD_PLOT_SECTIONS_ID
    MINVALUE 1
    MAXVALUE 99999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_PLOT_SECTIONS_INSERT
    BEFORE INSERT ON &&1..GNSD_PLOT_SECTIONS
    FOR EACH ROW
    BEGIN
        IF :new.ID IS NULL THEN
        SELECT &&1..SEQ_GNSD_PLOT_SECTIONS_ID.nextval INTO :new.ID FROM DUAL;
        END IF;
    END;
/
COMMIT
/


CREATE TABLE &&1..GNSD_SURROGATE_JOBS 
(
    ID NUMBER(8,0) NOT NULL ,
    JOBID INT ,
    USERID INT ,
    SURROGATEUSERID INT ,
    REQUESTDATE DATE ,
    REQUESTTYPE VARCHAR2(255) ,
    STOPAFTERPROCESS NUMBER (1,0),
    CONSTRAINT GNSD_SURROGATE_JOBS_PK PRIMARY KEY 
  (
    ID 
  )
    ENABLE 
)
/

CREATE SEQUENCE &&1..SEQ_GNSD_SURROGATE_JOBS_ID
    MINVALUE 1
    MAXVALUE 99999999
    START WITH 1
    INCREMENT BY 1
    NOCACHE
/

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_SURROGATE_JOBS_INSERT
    BEFORE INSERT ON &&1..GNSD_SURROGATE_JOBS
    FOR EACH ROW
    BEGIN
        IF :new.ID IS NULL THEN
        SELECT &&1..SEQ_GNSD_SURROGATE_JOBS_ID.nextval INTO :new.ID FROM DUAL;
        END IF;
    END;
/
COMMIT
/

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

CREATE OR REPLACE TRIGGER &&1..TRG_GNSD_PLACE_INSERT
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

CREATE OR REPLACE FORCE VIEW &&1..GNSD_GRIDJOBVIEW (
                                    "JOBID", "STATEDATE", "CREATEDATE", 
                                    "USERID", "REASONID", "REASON", 
                                    "JOBOUTPUT", "DEFINITION", "CUSTOM1", 
                                    "CUSTOM2", "CUSTOM3", "CUSTOM4", "CUSTOM5", 
                                    "CUSTOM6", "CUSTOM7", "CUSTOM8", 
                                    "CUSTOM9", "CUSTOM10", "STEP", 
                                    "STATUS", "NEEDSPROCESSING", "ISACTIVE", 
                                    "PROCESSID", "MACHINENAME", "PROCESSING_USERID", 
                                    "PROCESSOR_CLASSID", "ISARCHIVED", 
                                    "PERIODBEGINDATE", "PERIODENDDATE", "DESCRIPTION", 
                                    "PARCELNUMBER", "MUNICIPALITY", "CENTERAREAX", 
                                    "CENTERAREAY", "DOWNLOADCOUNT", "MAPEXTENTCOUNT", 
                                    "GEOATTACHMENTSENABLED", "DXFEXPORT", "SURROGATEUSERID", 
                                    "REQUESTDATE", "REQUESTTYPE", "STOPAFTERPROCESS", 
                                    "FIRSTNAME", "LASTNAME", "EMAIL")
AS
    SELECT GNSD_JOBS.JOBID,
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
    GNSD_JOBS.DXFEXPORT,
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

/
COMMIT
/

CREATE OR REPLACE FORCE VIEW &&1..MYREPRESENTATIVEJOBS (
                                    "JOBID", "JOBGUID", "REPRESENTATIVEUSERID", 
                                    "REPRESENTEDUSERID", "DOWNLOADCOUNT", "CREATEDDATE", 
                                    "REPRESENTATIVEUSERFIRSTNAME", "REPRESENTATIVEUSERLASTNAME", "REPRESENTATIVEUSERCOMPANY", 
                                    "REPRESENTATIVEUSEREMAIL", "REPRESENTEDUSERFIRSTNAME", "REPRESENTEDUSERLASTNAME", 
                                    "REPRESENTEDUSERCOMPANY", "REPRESENTEDUSEREMAIL", "ISARCHIVED", 
                                    "CUSTOM1", "CUSTOM2", "CUSTOM3", 
                                    "CUSTOM4", "CUSTOM5", "CUSTOM6", 
                                    "CUSTOM7", "CUSTOM8", "CUSTOM9", 
                                    "CUSTOM10", "PARCELNUMBER", "REASONID", "REASON", 
                                    "STATUS", "STEP")
    AS
SELECT    GNSD_JOBS.JOBID, 
          GNSD_JOBGUID.JOBGUID, 
          GNSD_SURROGATE_JOBS.SURROGATEUSERID AS REPRESENTATIVEUSERID, 
          GNSD_JOBS.USERID AS REPRESENTEDUSERID, 
          GNSD_JOBS.DOWNLOADCOUNT, 
          CREATEDDATE AS CREATEDDATE, 
          GNSD_USERS.FIRSTNAME AS REPRESENTATIVEUSERFIRSTNAME, 
          GNSD_USERS.LASTNAME AS REPRESENTATIVEUSERLASTNAME, 
          GNSD_USERS.COMPANY AS REPRESENTATIVEUSERCOMPANY, 
          GNSD_USERS.EMAIL AS REPRESENTATIVEUSEREMAIL, 
          GNSD_USERS_1.FIRSTNAME AS REPRESENTEDUSERFIRSTNAME, 
          GNSD_USERS_1.LASTNAME AS REPRESENTEDUSERLASTNAME, 
          GNSD_USERS_1.COMPANY AS REPRESENTEDUSERCOMPANY, 
          GNSD_USERS_1.EMAIL AS REPRESENTEDUSEREMAIL, 
          GNSD_JOBS.ISARCHIVED,
          GNSD_JOBS.CUSTOM1, GNSD_JOBS.CUSTOM2, GNSD_JOBS.CUSTOM3, 
          GNSD_JOBS.CUSTOM4, GNSD_JOBS.CUSTOM5, GNSD_JOBS.CUSTOM6, 
          GNSD_JOBS.CUSTOM7, GNSD_JOBS.CUSTOM8, GNSD_JOBS.CUSTOM9, 
          GNSD_JOBS.CUSTOM10, GNSD_JOBS.PARCELNUMBER, 
          GNSD_JOBS.REASONID, GNSD_REASONS.DESCRIPTION AS REASON,
          GNSD_JOBS.STATUS, GNSD_JOBS.STEP
FROM         
           &&1..GNSD_REASONS INNER JOIN
           &&1..GNSD_JOBS INNER JOIN
           &&1..GNSD_USERS GNSD_USERS_1 ON GNSD_JOBS.USERID = GNSD_USERS_1.USERID ON 
           &&1..GNSD_REASONS.REASONID = &&1..GNSD_JOBS.REASONID LEFT OUTER JOIN
           &&1..GNSD_JOBGUID ON GNSD_JOBS.JOBID = GNSD_JOBGUID.JOBID LEFT OUTER JOIN
           &&1..GNSD_USERS INNER JOIN
           &&1..GNSD_SURROGATE_JOBS ON GNSD_USERS.USERID = GNSD_SURROGATE_JOBS.SURROGATEUSERID ON 
           GNSD_JOBS.JOBID = GNSD_SURROGATE_JOBS.JOBID LEFT OUTER JOIN
               (SELECT JOBID AS JOBID, MIN(TIMESTAMP) AS CREATEDDATE
                 FROM &&1..GNSD_JOBS_LOG
                 GROUP BY JOBID) LOGS ON GNSD_JOBS.JOBID = LOGS.JOBID

/
COMMIT
/

SPOOL OFF
QUIT