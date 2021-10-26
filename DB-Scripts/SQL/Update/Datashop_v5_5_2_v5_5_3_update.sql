USE [$(geodatabase)]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Add status and step columns 
ALTER VIEW [$(gnsduser)].[MyRepresentativeJobs]
AS
SELECT gnsd_jobs.JobID, gnsd_jobGuid.JobGuid, gnsd_surrogate_jobs.SurrogateUserId AS RepresentativeUserId, 
           gnsd_jobs.UserID AS RepresentedUserId, gnsd_jobs.DownloadCount, Logs.CreatedDate, 
       gnsd_users.Firstname AS RepresentativeUserFirstname, gnsd_users.Lastname AS RepresentativeUserLastname, 
             gnsd_users.Company AS RepresentativeUserCompany, gnsd_users.Email AS RepresentativeUserEmail, 
             gnsd_users_1.Firstname AS RepresentedUserFirstname, gnsd_users_1.Lastname AS RepresentedUserLastname,              
             gnsd_users_1.Company AS RepresentedUserCompany, gnsd_users_1.Email AS RepresentedUserEmail, gnsd_jobs.IsArchived, 
             gnsd_jobs.Custom1, gnsd_jobs.Custom2, gnsd_jobs.Custom3, gnsd_jobs.Custom4, gnsd_jobs.Custom5, 
             gnsd_jobs.Custom6, gnsd_jobs.Custom7, gnsd_jobs.Custom8, gnsd_jobs.Custom9, gnsd_jobs.Custom10, 
             gnsd_jobs.ParcelNumber, gnsd_jobs.ReasonID, gnsd_reasons.Description AS Reason, 
             gnsd_jobs.Status, gnsd_jobs.Step
FROM         gnsd_reasons INNER JOIN
             gnsd_jobs INNER JOIN
             gnsd_users AS gnsd_users_1 ON gnsd_jobs.UserID = gnsd_users_1.UserID ON gnsd_reasons.ReasonID = gnsd_jobs.ReasonID LEFT OUTER JOIN
             gnsd_jobGuid ON gnsd_jobs.JobID = gnsd_jobGuid.JobId LEFT OUTER JOIN
             gnsd_users INNER JOIN
             gnsd_surrogate_jobs ON gnsd_users.UserID = gnsd_surrogate_jobs.SurrogateUserId ON 
             gnsd_jobs.JobID = gnsd_surrogate_jobs.JobId LEFT OUTER JOIN
                             (SELECT JobID AS JobId, MIN(Timestamp) AS CreatedDate
                               FROM gnsd_jobs_log
                               GROUP BY JobID) AS Logs ON gnsd_jobs.JobID = Logs.JobId
GO