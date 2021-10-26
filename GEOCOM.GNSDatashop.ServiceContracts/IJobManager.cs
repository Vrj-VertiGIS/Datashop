using System.Collections.Generic;
using System.ServiceModel;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.ErrorHandling.Exceptions;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;
using System;

namespace GEOCOM.GNSDatashop.ServiceContracts
{
    [ServiceContract]
    public interface IJobManager
    {
        /// <summary>
        /// This method first assosiates the given User with the given BizUser.
        /// Both are stored.
        /// an email is sent to the administrator.
        /// </summary>
        /// <param name="bizUser">
        /// The business bizUser to be created
        /// </param>
        /// <param name="user">
        /// The bizUser to be created
        /// </param>
        /// <returns>
        /// The ID of the User or -1 if something failed
        /// </returns>
        [OperationContract]
        long CreateBizUserAndSendAdminMail(BizUser bizUser, User user);

        [OperationContract]
        BizUser[] GetAllBizUsers();

        /// <summary>
        /// Gets a BizUser identified by his bizId
        /// if there are no results, null will be returned
        /// </summary>
        /// <param name="bizId">
        /// The BusinessUser Id
        /// </param>
        /// <returns>
        /// The BusinessUser corresponding to the Id
        /// </returns>
        [OperationContract]
        BizUser GetBizUserByBizId(long bizId);

        /// <summary>
        /// Gets a BizUser identified by his password reset id
        /// if there are no results, null will be returned
        /// </summary>
        [OperationContract]
        BizUser GetBizUserByPasswordResetId(Guid pwdResetId);

        [OperationContract]
        User[] GetUserNotActivated();

        [OperationContract]
        User[] GetUserNotActivatedOrdered(string orderExpression, bool orderAscending);

        /// <summary>
        /// This method takes a userId (NOT a BizUserId) and activates this bizUser.
        /// A Mail will be sent to the bizUser.
        /// </summary>
        /// <param name="userId">
        /// A userId to be activated
        /// </param>
        /// <returns>
        /// True if successful, false otherwise
        /// </returns>
        [OperationContract]
        bool ActivateBizUserAndSendNotificationMail(long userId);

        [OperationContract]
        bool UpdateBizUser(BizUser bizUser);

        [OperationContract]
        bool DeleteBizUser(BizUser bizUser, bool deleteAllRelatedData, bool deleteReleatedUser);

        [OperationContract]
        bool DeleteBizUserByUserId(long userId, bool deleteAllRelatedData, bool deleteReleatedUser);

        [OperationContract]
        bool SendResetPasswordMail(User user);

        /// <summary>
        /// Creates the temp user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(TempUserTooManyRequestsFault))]
        long CreateTempUser(User user);

        [OperationContract]
        long CreateUser(User user);

        /// <summary>
        /// Deletes a bizUser, only if he has no jobs
        /// </summary>
        /// <param name="userId">The userId of the bizUser to be deleted</param>
        [OperationContract]
        void DeleteUser(long userId);

        [OperationContract]
        User GetUser(long userId);

        [OperationContract]
        User[] GetUserByEmail(string email);

        [OperationContract]
        User[] GetAllUsers();

        [OperationContract]
        User[] GetAllUsersPaged(int skip, int take);

        /// <summary>
        /// Gets all Users in the Database ordered by Property defined in String Orderexpression
        /// </summary>
        /// <param name="orderExpression">
        /// The property to sort
        /// </param>
        /// <param name="ascending">
        /// True = asscending, false = descending
        /// </param>
        /// <param name="showBusinessUser">
        /// Indicates if BusinessUsers should be returner
        /// </param>
        /// <param name="showTempUser">
        /// Indicates if TempUsers should be returned
        /// </param>
        /// <returns>
        /// All User in DB as an Array, sorted.
        /// </returns>
        [OperationContract]
        User[] GetAllUsersSorted(string orderExpression, bool ascending, bool showBusinessUser, bool showTempUser);

        [OperationContract]
        User[] GetSomeUsersSorted(string orderExpression, bool ascending, int index, int quantity, bool showBusinessUser, bool showTempUser);

        /// <summary>
        /// Search in Users and get a sorted result with paging
        /// </summary>
        /// <param name="salutation">
        /// The salutation to be searched
        /// </param>
        /// <param name="firstname">
        /// The firstname to be searched
        /// </param>
        /// <param name="lastname">
        /// The lastname to be searched
        /// </param>
        /// <param name="street">
        /// The street to be searched
        /// </param>
        /// <param name="streetnr">
        /// The streetnr to be searched
        /// </param>
        /// <param name="citycode">
        /// The citycode to be searched
        /// </param>
        /// <param name="city">
        /// The city to be searched
        /// </param>
        /// <param name="company">
        /// The company to be searched
        /// </param>
        /// <param name="email">
        /// The email to be searched
        /// </param>
        /// <param name="tel">
        /// The tel to be searched
        /// </param>
        /// <param name="fax">
        /// The fax to be searched
        /// </param>
        /// <param name="status">
        /// The status to be searched
        /// </param>
        /// <param name="roles">
        /// The roles to be searched
        /// </param>
        /// <param name="userId">
        /// If userId is not empty, maximal 1 result will be returned
        /// </param>
        /// <param name="showBusinessUser">
        /// The showBusinessUser to be searched
        /// </param>
        /// <param name="showTempUser">
        /// The showTempUser to be searched
        /// </param>
        /// <param name="orderExpression">
        /// Sort Expression
        /// </param>
        /// <param name="ascending">
        /// True = asscending, false = descending
        /// </param>
        /// <param name="index">
        /// The start Index
        /// </param>
        /// <param name="quantity">
        /// Amount of results
        /// </param>
        /// <returns>
        /// This Method will return all Users that have matching substrings in all not empty fields 
        /// </returns>
        [OperationContract]
        User[] SearchUsers(string salutation, string firstname, string lastname, string street, string streetnr, string citycode, string city, string company, string email, string tel, string fax, string status, string roles, long? userId, bool showBusinessUser, bool showTempUser, string orderExpression, bool ascending, int index, int quantity);

        /// <summary>
        /// Updates a User in the Database
        /// </summary>
        /// <param name="user">The bizUser to be updated</param>
        [OperationContract]
        void UpdateUser(User user);

        [OperationContract]
		[FaultContract(typeof(TooManyPlotsFault))]
        long CreateJob(Job job);

        [OperationContract]
        long CloneJob(long jobId);


        /// <summary>
        /// Checks whether creation of a new job is allowed for the specified user.
        /// </summary>
        /// <param name="userId">The userId of the user to be checked</param>
        /// <param name="jobCountLimit">The maximum number of jobs allwed in the time period</param>
        /// <param name="timePeriod">The time period in days</param>
        /// <returns>True if a new job may be created, False otherwise</returns>
        [OperationContract]
        bool IsJobCreationAllowed(long userId, int jobCountLimit, int timePeriod);

        [OperationContract]
        void UpdateJob(Job job);

        [OperationContract]
        void RestartJobFromStep(long jobId, int workflowStepIndex);

        [OperationContract]
        int[] GetAllRestartableStepIds(long jobId);

        [OperationContract]
        WorkflowStepIdName[] GetAllStepIdNames(long jobId);

        [OperationContract]
        WorkflowStepIdName[] GetAllRestartableStepIdNames(long jobId);

        [OperationContract]
        WorkflowStepIdName GetNextStepIdName(long jobId);

        [OperationContract]
        string GetWorkflowStepNameById(long jobId, int workflowStepId);

        [OperationContract]
        void ActivateJob(long jobId);

        /// <summary>
        /// Adds a new log in the Database. You can specify JobId, Status, SubStatus and Message.
        /// This Methode is mostly used to allow userdefined logentries for a job
        /// </summary>
        /// <param name="jobId">The jobId of the changed job</param>
        /// <param name="message">The message for the log entry</param>
        [OperationContract]
        void AddJobLogByJobId(long jobId, string message);

        [OperationContract]
        void DeleteJob(long jobId);

        [OperationContract]
        Job GetJobByGuid(string jobGuid);

        [OperationContract]
        JobDetails GetJobDetailsById(long jobId);

        [OperationContract]
        Job GetJobById(long jobId);

        /// <summary>
        /// Gets all GNSD_gridjobs for a specific bizUser
        /// </summary>
        /// <param name="userId">
        /// The userid
        /// </param>
        /// <returns>
        /// All jobs associated with this bizUser
        /// </returns>
        [OperationContract]
        JobDetails[] GetJobForUser(long userId);

        /// <summary>
        /// Returns all Jobs that are associated with this email-adress
        /// </summary>
        /// <param name="email">The email address to be searched</param>
        /// <returns>All jobs for the specified email address</returns>
        [OperationContract]
        Job[] GetJobForEmail(string email);

        [OperationContract]
        Job[] GetAllJobs();

        [OperationContract]
        Job[] GetAllJobsSorted(string orderExpression, bool ascending);

        [OperationContract]
        JobDetails[] GetJobDetailsSorted(bool showArchived, bool showNotArchived, string orderExpression, bool ascending, int startIndex, int quantity, string whereClause);

        [OperationContract]
        JobDetails[] SearchJobs(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool showArchived, bool showNotArchived, string orderExpression, bool ascending, int index, int quantity, string whereClause);

        [OperationContract]
        JobDetails[] SearchJobsTyped(long? jobId, DateTime? createDateOld, DateTime? createDateNew, DateTime? stateDateOld, DateTime? stateDateNew, string reason, string status, string free1, string free2, bool showArchived, bool showNotArchived, string orderExpression, bool ascending, int index, int quantity, string whereClause);
        
        [OperationContract]
        Reason[] GetReasons();

        [OperationContract]
        KeyValuesPair[] GetScales();

        [OperationContract]
        PlotFormatDef[] GetTemplatesForUser(long userId);

        [OperationContract]
        Plotdefinition[] GetAllTemplates();

        [OperationContract]
        bool UpdateTemplate(Plotdefinition plotTemplate);

        [OperationContract]
        bool DeleteTemplate(Plotdefinition plotTemplate);

        [OperationContract]
        void CreateDbSchema();

        [OperationContract]
        List<ProfileInfo> GetProfileInfo();

        [OperationContract]
        DxfExportInfo GetDxfExportInfo(string name);

        [OperationContract]
        List<DxfExportInfo> GetDxfExportInfos();

        /// <summary>
        /// Authenticates a bizUser.
        /// The bizUser is authenticated depending on the userRole.
        /// If the bizUser is authenticated, and not already existing, a new bizUser will be created.
        /// The password has to be provided for the userRoles ADMIN and BUSINESS.
        /// For the other userRoles the password is ignored.
        /// </summary>
        /// <param name="userIdentification">The bizUser identification (e.g. SessionId for a public bizUser, email for a BUSINESS bizUser)</param>
        /// <param name="userRole">The bizUser role</param>
        /// <param name="password">The password only used for userRoles ADMIN and BUSINESS</param>
        /// <returns>The UserProfile of the bizUser</returns>
        [OperationContract]
        UserProfile AuthenticateUser(string userIdentification, UserRole userRole, string password);

        [OperationContract]
        PlotSection[] GetAllPlotSections();

        [OperationContract]
        WorkflowStepState GetJobsStepStateByJobId(long jobId);

        [OperationContract]
        List<MyJob> GetMyJobsByUserId(long userId, bool isRepresentativeUser, string sortExpression, bool sortAscending);

        [OperationContract]
        List<MyJob> GetMyJobsByUserIdAndSearchParameters(long userId, bool isRepresentativeUser, MyJobSearchParameters parameters, string sortExpression, bool sortAscending);

        /// <summary>
        /// Gets all the PlacementOptions.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<PlacementOption> GetAllPlacementOptions();

        /// <summary>
        /// Gets the users by surrogate filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="skip">Skip first n elements</param>
        /// <param name="take">Take n elements</param>
        /// <returns></returns>
        [OperationContract]
        User[] GetUsersBySurrogateFilterPaged(string filter, int skip, int take);

        /// <summary>
        /// Gets the data export types.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<string> GetDataExportTypes();

        /// <summary>
        /// Let job manager know that a login attempt failed.
        /// </summary>
        [OperationContract]
        void SetFailedLogin(long userid);

        /// <summary>
        /// Let job manager know that a login attempt succeeded.
        /// </summary>
        [OperationContract]
        void ResetFailedLogin(long userid);
    }

	/// <summary>
	/// This class bears data of exception that occurs when there is too many plot request per minute
	/// </summary>
	public class TooManyPlotsFault
	{
		/// <summary>
		/// The actual amount of plots in last minute by a user
		/// </summary>
		public int PlotCountInLastMinute { get; set; }

		/// <summary>
		/// The maximal allowed limit of plots
		/// </summary>
		public int ConfiguredMaxPlotsRate { get; set; }
	}
}
