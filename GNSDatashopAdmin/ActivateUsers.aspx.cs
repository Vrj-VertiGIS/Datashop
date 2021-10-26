using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Core.Service;

namespace GNSDatashopAdmin
{
    /// <summary>
    /// Codebehind for the Activate Users page
    /// </summary>
    public partial class ActivateUsers : Page
    {
        #region Private members

        /// <summary>
        /// 
        /// </summary>
        private IMsg log = new Msg(typeof (ActivateUsers));

        #endregion

        #region Page Lifecycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ProcessRequest();
            }
        } 

        #endregion

        #region UI Control Events

        /// <summary>
        /// Handles the RowCommand event of the BizGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
        protected void BizGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            lblStatus.Text = "";

            //look for custom events
            if (e.CommandName.Equals("Activate"))
                Activate(e);

            if (e.CommandName.Equals("Reject"))
                RejectUser(e);
        } 

        #endregion

        #region Private Methods

        private void ProcessRequest()
        {
            try
            {
                lblStatus.Text = "";
                FillGridview();
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = string.Format("There was an error loading the page: {0}", ex.Message);

                log.Error("There was an error loading the activate users page", ex);
            }
        }

        /// <summary>
        /// Rejects the user.
        /// </summary>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
        private void RejectUser(GridViewCommandEventArgs e)
        {
            long userID = -1;

            try
            {
                if (long.TryParse(e.CommandArgument.ToString(), out userID))
                {
                    var user = DatashopService.Instance.JobService.GetUser(userID);
                    if (DatashopService.Instance.JobService.DeleteBizUserByUserId(userID, false, true))
                        lblStatus.Text = String.Format("User {0} ({1} {2}) was removed.", userID, user.FirstName, user.LastName);
                    else
                        lblStatus.Text = "The user could not be removed. Possibly he has already ordered some jobs.";
                }
                else
                    lblStatus.Text = "The user could not be deleted.";
            }
            catch (Exception ex)
            {
                log.ErrorFormat("There was an error deleting the user with Id: {0}", ex, userID);

                lblStatus.Text = string.Format("There was an error deleting the user with Id {0}:  {1}", userID, ex.Message);
            }

            FillGridview();
        }

        /// <summary>
        /// Activates the specified e.
        /// </summary>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
        private void Activate(GridViewCommandEventArgs e)
        {
            long userID = -1;

            try
            {
                if (long.TryParse(e.CommandArgument.ToString(), out userID))
                {
                    if (DatashopService.Instance.JobService.ActivateBizUserAndSendNotificationMail(userID))
                    {
                        var user = DatashopService.Instance.JobService.GetUser(userID);
                        lblStatus.Text = String.Format("User {0} ({1} {2}) was activated.", userID, user.FirstName, user.LastName);
                    }
                    else
                        lblStatus.Text = "The user could not be activated.";
                }
                else
                    lblStatus.Text = "The user could not be activated.";
            }
            catch (Exception ex)
            {
                log.ErrorFormat("There was an error activating the user with Id: {0}", ex, userID);

                lblStatus.Text = string.Format("There was an error activating the user with Id {0}:  {1}", userID, ex.Message);
            }

            FillGridview();
        }

        /// <summary>
        /// Fills the gridview.
        /// </summary>
        private void FillGridview()
        {
            var bizUsers = DatashopService.Instance.JobService.GetUserNotActivated();

            BizGrid.DataSource = bizUsers;
            BizGrid.DataBind();
        } 

        #endregion
    }
} 