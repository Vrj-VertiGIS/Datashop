using System;
using System.ServiceModel;
using GEOCOM.GNSDatashop.Model.JobData;

namespace GEOCOM.GNSDatashop.ServiceContracts
{
    [ServiceContract]
    public interface IAdminDatashop
    {
        [OperationContract]
        int GetJobCount(bool showArchived, bool showNotArchived, string whereClause);

        [OperationContract]
        JobLog[] GetLogsForJob(long jobId);

        [OperationContract]
        int GetUserCount(bool showBusinessUser, bool showTempUser);

        [OperationContract]
        int GetUserSearchCount(string adress, string firstname, string lastname, string street, string streetnr, string citycode, string city, string company, string email, string tel, string fax, string status, string roles, long? id, bool showBusinessUser, bool showTempUser);

        [OperationContract]
        int GetJobSearchCount(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool showArchived, bool showNotArchived, string whereClause);

        [OperationContract]
        bool ArchiveJob(long jobId);

        /// <summary>
        /// Dis Method allows archiving of multiple jobs.
        /// </summary>
        /// <param name="time">
        /// A DateTime, every job older than this time will be archived 
        /// </param>
        /// <returns>
        /// For every job there is a String with the results, the last string is a summery 
        /// </returns>
        [OperationContract]
        string[] ArchiveJobsByTime(DateTime time);
    }
}