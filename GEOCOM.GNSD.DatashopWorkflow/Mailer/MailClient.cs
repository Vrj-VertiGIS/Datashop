using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.Mail;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DatashopWorkflow.GeoDataBase;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.DatashopWorkflow.Mailer
{
    public class MailClient
    {
        // private ref to parent log and config 
        private IMsg _log;

        // variables to include in the email templates
        private Dictionary<string, string> _variables = new Dictionary<string, string>();

        public MailClient(IMsg log)
        {
            _log = log;
        }

        /// <summary>
        /// Use this constructor to add additional textvariables 
        /// A variable will be replaced if the pattern %(key) is found
        /// i.E. for key="name" and value="Müller" the fragment %(name) is replaced by "Müller"
        /// </summary>
        public MailClient(IMsg log, Dictionary<string, string> variables)
            : this(log)
        {
            if (variables != null)
                _variables = variables;
        }

        internal void SendPlotFinishedMail(JobGuid jobGuid, User toUser, bool isSurrogateJob)
        {
            try
            {
                string downloadUrl = GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["confirmJob"].DownloadUrl;
                downloadUrl = Utils.Utils.ReplaceVars(downloadUrl, _variables);

                string finalLink = string.Format("<a href=\"{0:s}\">{1:s}</a>", downloadUrl, downloadUrl);
                _variables["link_download"] = finalLink;

                // Send the mail synchronous
                MailSender mail = new MailSender();
                if (isSurrogateJob)
                {
                    mail.SendMail("confirmSurrogateJob", toUser.Email, _variables, false);
                }
                else
                {
                    mail.SendMail("confirmJob", toUser.Email, _variables, false);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat(string.Format("Error sending email for job {0}", jobGuid.JobId), ex);
                throw;
            }
        }

        internal void SendDataOwnerNotifyMail(long jobId, DataOwner dataOwner, User reqUser, IList<string> extentDescriptions)
        {
            try
            {
                _variables["owner_description"] = dataOwner.Description;
                _variables["owner_email"] = dataOwner.EMail;
                _variables["owner_id"] = dataOwner.OwnerId.ToString();

                StringBuilder extent = new StringBuilder();
                foreach (string extentdesc in extentDescriptions)
                {
                    extent.Append(extentdesc);
                    extent.Append(", ");
                }

                string extentString = extent.ToString().Trim();
                _variables["extent"] = extentString.Substring(0, extentString.Length - 1);

                // Send the mail synchronous
                MailSender mail = new MailSender();
                mail.SendMail("restrictedAreaAdmin", dataOwner.EMail, _variables, false);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat(string.Format("Error sending Admin-email for job {0}", jobId), ex);
                throw;
            }
        }

        /// <summary>
        /// Send an email according the 'invalidMapLayers' template.
        /// </summary>
        internal void SendInvalidMapLayersMail(long jobId, string invalidLayersNames, string mapFileMxdPath)
        {
            _variables["invalid_layers"] = invalidLayersNames;
            _variables["mxd_path"] = mapFileMxdPath;
            _variables["job_id"] = jobId.ToString(CultureInfo.InvariantCulture);

            string toEmail = GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["invalidMapLayers"].To;
            MailSender mailSender = new MailSender();
            mailSender.SendMail("invalidMapLayers", toEmail, _variables, false);
        }

		/// <summary>
		/// Sends an email using 'jobStoppedAfterStep' template (in the xml configuration).
		/// </summary>
		/// <param name="stepStopCriterion">The step stop criterion from xml configuration.</param>
		/// <param name="mailRecipients">The comma-separated email addresses.</param>
		internal void SendJobStoppedAfterStepMail(StopCriterion stepStopCriterion, string mailRecipients)
		{
			_variables["stopped_after_step"] = stepStopCriterion.StopAfterStepName;
			MailSender mailSender = new MailSender();

			foreach (var email in mailRecipients.Split(','))
			{
				mailSender.SendMail("jobStoppedAfterStep", email, _variables, false);
			}
		}

        /// <summary>
        /// Sends an email using 'maxGeoAttachemntSizeExceeded' template (in the xml configuration).
        /// </summary>
        /// <param name="maxSizeMB">The maximal size in megabytes MB.</param>
        /// <param name="actualSizeMB">The actual size in magebytes MB.</param>
        internal void SendMaxGeoAttachemntSizeExceeded(string maxSizeMB, string actualSizeMB)
        {
            _variables["geoattachments_max_size"] = maxSizeMB;
            _variables["geoattachments_actual_size"] = actualSizeMB;
            string toEmail = GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["maxGeoAttachemntSizeExceeded"].To;
            MailSender mailSender = new MailSender();
            mailSender.SendMail("maxGeoAttachemntSizeExceeded", toEmail, _variables, false);
        }
    }
}
