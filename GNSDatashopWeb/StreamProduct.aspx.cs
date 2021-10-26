using System;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Common;
using GEOCOM.GNSD.Web.Core.DocumentStreaming;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind for the StreamProduct page
    /// </summary>
    public partial class StreamProduct : Page
    {
        #region Private members

        /// <summary>
        /// holds the job for this instance
        /// </summary>
        private Job _job;

        /// <summary>
        /// holds the user for this instance
        /// </summary>
        private User _user;

        /// <summary>
        /// Holds the logger for this instance
        /// </summary>
        private readonly IMsg _log = new Msg(typeof(StreamProduct)); 

        #endregion

        #region MyRegion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            hfdJobID.Value = Request.Params["jobid"];
            _log.Debug("Page for job " + hfdJobID.Value + " is loading");

            try
            {
                var jobGuid = hfdJobID.Value;
                _job = DatashopService.Instance.JobService.GetJobByGuid(jobGuid);

                //NOTE: there's no other way to do this as the view that provides the job details doesn't have the guid field
                //NOTE: and we're not supposed to make any alterations to the data model
                var jobDetails = DatashopService.Instance.JobService.GetJobDetailsById(_job.JobId);

                this.HandlePotentialDocumentExpiry(jobDetails);

                _user = DatashopService.Instance.JobService.GetUser(_job.UserId);

                lblJobId.Text = string.Format("{0}: {1}", WebLanguage.LoadStr(3014, "Job-ID"), _job.JobId);
                lblUserID.Text = String.Format("{0}: {1:d} ({2:s} {3:s})", WebLanguage.LoadStr(3107, "UserID"), _user.UserId, _user.FirstName, _user.LastName);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0} tried to download job with id: '{1}' but failed. Exceptionmessage:\n{2}", Request.Params["REMOTE_ADDR"], hfdJobID.Value, ex.Message), ex);

                Response.RedirectSafe("error/GeneralErrorPage.aspx", false);
            }
        } 

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Handles the potential expiry of a document by hiding the download link and replacing it with a link to return to the application.
        /// </summary>
        /// <param name="jobDetails">The job details.</param>
        private void HandlePotentialDocumentExpiry(JobDetails jobDetails)
        {
            if (jobDetails == null)
                throw new ArgumentNullException("jobDetails");

            if (DatashopWebConfig.Instance.DocumentExpiry.Enabled)
            {
                if (jobDetails.CreateDate != null)
                {
                    var expiryTimeSpan = new TimeSpan(DatashopWebConfig.Instance.DocumentExpiry.ArchiveAfterDays, 0, 0, 0);

                    var expiryDate = jobDetails.CreateDate.Value.Add(expiryTimeSpan);

                    var expired = DateTime.Now > expiryDate;

                    this.lbtDownload.Visible = this.litReadyForDownload.Visible = !expired;
                    this.lnkReturnToApp.Visible = this.litExpired.Visible = expired;
                }
            }
            else
                this.lnkReturnToApp.Visible = this.litExpired.Visible = false;
        }

        /// <summary>
        /// Updates the job info and log.
        /// </summary>
        private void UpdateJobInfoAndLog()
        {
            DatashopService.Instance.JobService.AddJobLogByJobId(_job.JobId, string.Format("Der Benutzer hat die Dokumente nach {0} abgeholt.", Request.Params["REMOTE_ADDR"]));

            _log.Info(string.Format("User {0} ({1}, {2}) did download job {3} to {4}", _user.UserId, _user.FirstName, _user.LastName, _job.JobId, Request.Params["REMOTE_ADDR"]));
        } 

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [click LBT download].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnClickLbtDownload(object sender, EventArgs e)
        {
            if (_job != null)
            {
                var streamer = new DocumentStreamer(_job, Response);
                streamer.CopyDocumentToRespose();

                UpdateJobInfoAndLog();
            }
        } 

        #endregion
    }
}