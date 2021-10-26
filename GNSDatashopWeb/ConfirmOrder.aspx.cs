using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Common;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind for the order confirmation page
    /// </summary>
    public partial class ConfirmOrder : Page
    {
        #region Page Lifecycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.ProcessRequest();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        private void ProcessRequest()
        {
            var jobId = Convert.ToInt64(Session["jobid"]);

            var jobInfo = DatashopService.Instance.JobService.GetJobById(jobId);

            if (jobInfo == null) return;

            var emails = new List<string>();
            if (jobInfo.SurrogateJob != null &&
                (DatashopWebConfig.Instance.RepresentativeJob.Recipient == RepresentativeJobRecipient.Selected ||
                 DatashopWebConfig.Instance.RepresentativeJob.Recipient == RepresentativeJobRecipient.Both))
            {
                var userInfo = DatashopService.Instance.JobService.GetUser(jobInfo.SurrogateJob.UserId);
                emails.Add(userInfo.Email);
            }
            if (DatashopWebConfig.Instance.RepresentativeJob.Recipient == RepresentativeJobRecipient.Representative ||
                DatashopWebConfig.Instance.RepresentativeJob.Recipient == RepresentativeJobRecipient.Both)
            {
                var userId = Convert.ToInt64(Session["userid"]);
                var userInfo = DatashopService.Instance.JobService.GetUser(userId);
                emails.Add(userInfo.Email);

            }

            if (!emails.Any()) return;

            this.DisplayJobInfo(string.Join(",", emails), jobId);
        }

        /// <summary>
        /// Displays the job info.
        /// </summary>
        /// <param name="userInfo">The user info.</param>
        /// <param name="jobId">The job id.</param>
        private void DisplayJobInfo(string emails, long jobId)
        {
            lblJobID.Text = string.Format("{0}: {1}", WebLanguage.LoadStr(3014, "Job-ID"), jobId);

            lblUserEMail.Text = string.Format(WebLanguage.LoadStr(3003, "An email will be sent to <b>{0}</b> containing a link to download the map."),
                emails);
            SetRequestPageLink();
        }

        #endregion

        #region Control Events


        protected void SetRequestPageLink()
        {
            var pageInfo = DatashopWebConfig.Instance.DefaultRequestPage;

            if (pageInfo != null)
            {
                hypPlotRequest.Text = WebLanguage.LoadStr(pageInfo.TextId, "New Plot Request");
                hypPlotRequest.NavigateUrl = pageInfo.PageName;
            }
        }

        #endregion
    }
}