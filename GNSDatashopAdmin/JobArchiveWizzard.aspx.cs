using System;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using System.Web.UI;

namespace GNSDatashopAdmin
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Page" />
    public partial class JobArchiveWizzard : Page
    {
        #region Page Lifecycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            lblArchSum.Text = "";
            lblCountResult.Text = "";
            ResultList.Items.Clear();
        } 

        #endregion

        protected void DateTimeValidator(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Equals(""))
            {
                args.IsValid = false;
                return;
            }
            DateTime a;
            args.IsValid = DateTime.TryParse(args.Value, out a);
        }

        protected void Archive(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            try
            {
                JobDetails[] jobsDetails = GetJobDetails();

                int archivedJobs = 0;
                foreach (JobDetails jobDetail in jobsDetails)
                {
                    if (!jobDetail.ProcessorClassId.Equals(WorkflowDefinitions.AdminWorkflowClassId))
                    {
                        DatashopService.Instance.JobService.RestartJobFromStep(jobDetail.JobId, 7);
                        archivedJobs++;
                    }
                }

                lblArchSum.Text = archivedJobs + " jobs have been submited to archive.";
            }
            catch (Exception exp)
            {

                lblArchSum.Text = "An error occoured: " + exp.Message;
            }
           
        }

        protected void Count(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            var date = DateTime.Parse(txtDate.Text);
            
            JobDetails[] jobsDetails = GetJobDetails();

            int jobsToArchive = 0;
            foreach (JobDetails jobDetail in jobsDetails)
            {
                if (!jobDetail.ProcessorClassId.Equals(WorkflowDefinitions.AdminWorkflowClassId))
                {
                    jobsToArchive++;
                }
            }

            lblCountResult.Text = jobsToArchive + " jobs were created before " + date.ToShortDateString() + " " + date.ToShortTimeString() +
                                  " and are not yet archived.";

        }

        private JobDetails[] GetJobDetails()
        {
            JobDetails[] jobsDetails = DatashopService.Instance.JobService.SearchJobs(
              null,
              string.Empty,
              txtDate.Text,
              string.Empty,
              string.Empty,
              string.Empty,
              string.Empty,
              string.Empty,
              string.Empty,
              false,
              true,
              "JobId",
              false,
              0,
              int.MaxValue,
              null);

            return jobsDetails;
        }
    }
}
