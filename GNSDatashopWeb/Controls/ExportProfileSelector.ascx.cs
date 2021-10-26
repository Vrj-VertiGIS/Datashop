using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using OutputFormat = GEOCOM.GNSD.Common.Model.OutputFormat;

namespace GEOCOM.GNSD.Web.Controls
{
   /// <summary>
    /// This encapsulates the export profile selector, the profile description and the export format selector
    /// </summary>
    public partial class ExportProfileSelector : RequestUserControl
    {
        #region Public Properties

        /// <summary>
        /// Gets the profile.
        /// </summary>
        public string Profile
        {
            get { return this.GetProfile(); }
        }

        /// <summary>
        /// Gets the output format.
        /// </summary>
        public OutputFormat OutputFormat
        {
            get { return this.GetOutputFormat(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is DXF.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is DXF; otherwise, <c>false</c>.
        /// </value>
        public bool IsDxf { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid 
        {
            get { return this.Validate(); }
        }

       #endregion

       #region Page Lifecycle Events

       /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
       protected void Page_Load(object sender, EventArgs e)
       {
              this.SetFieldFromConfig(UpdatePanel1);

              if (!IsPostBack)
              {
                  this.DisableFields();
                  this.PopulateExportTypes();
              }
       }

        #endregion

        #region Private Methods

       /// <summary>
       /// Disables the fields.
       /// </summary>
       private void DisableFields()
       {
           this.cboProfile.DropDown.Enabled = false;
           this.cboDataFormat.DropDown.Enabled = false;
           this.cboDxfExport.DropDown.Enabled = false;

           this.cboProfile.Required = false;
           this.cboDataFormat.Required = false;
           this.cboDxfExport.Required = false;
       }

       /// <summary>
       /// Resets this instance.
       /// </summary>
       private void Reset()
       {
           this.cboProfile.DropDown.Items.Clear();
           this.cboDataFormat.DropDown.Items.Clear();
           this.cboDxfExport.DropDown.Items.Clear();
       }

       /// <summary>
       /// Populates the export types.
       /// </summary>
       private void PopulateExportTypes()
       {
            var profiles = DatashopService.Instance.JobService.GetDataExportTypes();

            this.cboExportType.DropDownItems.Add(new ListItem { Text = WebLanguage.LoadStr(3966, "Please select an export type"), Value = "-" });

            this.cboExportType.DropDownItems.AddRange(
                profiles.ConvertAll(s => new ListItem { Value = s, Text = s })
                .ToArray()
             );

            if (cboExportType.DropDownItems.Count == 2)
            {
                cboExportType.DropDown.Items[1].Selected = true;

                cboExportType.DropDown.Enabled = false;

                switch(cboExportType.DropDown.Items[1].Value)
                {
                    case "DXF":
                        this.SetForDXF();
                        break;
                    case "TDE":
                        this.SetForTDE();
                        break;
                }
            }
        } 

       /// <summary>
        /// Gets the output format.
        /// </summary>
        /// <returns></returns>
        private OutputFormat GetOutputFormat()
        {
            return string.IsNullOrEmpty(cboProfile.DropDown.SelectedValue)
                ? OutputFormat.None
                : (OutputFormat)Enum.Parse(typeof(OutputFormat), cboDataFormat.DropDown.SelectedValue);
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <returns></returns>
        private string GetProfile()
        {
            return this.cboExportType.SelectedItem.Value == "DXF"
                ? this.cboDxfExport.DropDown.SelectedValue
                : this.cboProfile.DropDown.SelectedValue;
        }

        /// <summary>
        /// Sets the control attributes.
        /// </summary>
        /// <param name="isTDE">if set to <c>true</c> [is TDE].</param>
        private void SetControlAttributes(bool isTDE)
        {
            this.cboProfile.Visible = this.cboProfile.Required = this.cboProfile.DropDown.Enabled = isTDE;
            this.cboDataFormat.Visible = this.cboDataFormat.Required = this.cboDataFormat.DropDown.Enabled = isTDE;
            this.cboDxfExport.Visible = this.cboDxfExport.Required = this.cboDxfExport.DropDown.Enabled = !isTDE;

            this.IsDxf = !isTDE;
        }

        /// <summary>
        /// Sets for TDE.
        /// </summary>
        private void SetForTDE()
        {
            this.SetControlAttributes(true);

            if (!this.LoadCboProfile())
                this.DisableProfileSelector();
        }

        /// <summary>
        /// Sets for DXF.
        /// </summary>
        private void SetForDXF()
        {
            this.SetControlAttributes(false);

            this.lblProfileDetail.Visible = false;
            this.lblProfileDetail.Text = string.Empty;

            var infos = DatashopService.Instance.JobService.GetDxfExportInfos();

            this.cboDxfExport.DropDownItems.Clear();

            this.cboProfile.DropDownItems.Clear();

            this.IsDxf = true;

            this.cboDxfExport.DropDownItems.Add(new ListItem(WebLanguage.LoadStr(3963, "Please select a DXF profile"), "0"));

            infos.ForEach(dxf => this.cboDxfExport.DropDownItems.Add(new ListItem(dxf.Name, dxf.Name)));

            if (this.cboDxfExport.DropDownItems.Count == 2)
            {
                this.cboDxfExport.DropDown.Items[1].Selected = true;

                this.CboDxfProfileSelectedIndexChanged(this.cboDxfExport, EventArgs.Empty);

                this.cboDxfExport.DropDown.Enabled = false;
            }
        }

       /// <summary>
        /// Loads the cbo profile.
        /// </summary>
        /// <returns></returns>
        private bool LoadCboProfile()
       {
           this.SetControlAttributes(true);

            var success = true;

            try
            {
                LogDebug("loading profiles");

                cboProfile.DropDown.Items.Clear();

                cboProfile.DropDown.Items.Add(new ListItem(WebLanguage.LoadStr(3803, "Please select a profile"), "0"));

                var profileInfos = DatashopService.Instance.JobService.GetProfileInfo();

                profileInfos.ForEach(p => cboProfile.DropDown.Items.Add(new ListItem(p.Name, p.Guid)));

                if (this.cboProfile.DropDown.Items.Count == 2)
                {
                    this.cboProfile.DropDown.Items[1].Selected = true;

                    this.CboProfileSelectedIndexChanged(this.cboProfile, EventArgs.Empty);

                    this.cboProfile.DropDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Loading profiles. Is there a configuration error?: {0}", ex.Message), ex);

                success = false;
            }

            return success;
        }

        /// <summary>
        /// Loads the cbo data format.
        /// </summary>
        /// <param name="profileGuid">The profile GUID.</param>
        /// <returns></returns>
        private bool LoadCboDataFormat(string profileGuid)
        {
            this.SetControlAttributes(true);

            var success = true;

            try
            {
                LogDebug("loading profiles");

                var profileInfos = DatashopService.Instance.JobService.GetProfileInfo();

                var profile = profileInfos.First(p => p.Guid.ToString() == profileGuid);

                if (profile == null)
                    return false;

                cboDataFormat.DropDown.Items.Add(new ListItem(this.LoadStr(3957, "Please select a db format"), "0"));

                this.lblProfileDetail.Visible = true;
                lblProfileDetail.Text = this.CleanDescription(profile.Description);

                profile.TargetFormat.ForEach(t => cboDataFormat.DropDown.Items.Add(new ListItem(t.ToString())));

                if (this.cboDataFormat.DropDown.Items.Count == 2)
                {
                    this.cboDataFormat.DropDown.Items[1].Selected = true;

                    this.cboDataFormat.DropDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Loading profiles. Is there a configuration error?: {0}", ex.Message), ex);

                success = false;
            }

            return success;
        }

        /// <summary>
        /// Cleans the description.
        /// this description contains new line and tab characters.
        /// we remove the tabs and replace the new lines by some <br/> html tags
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        private string CleanDescription(string description)
        {
            // remove the tabs
            var s = description.Replace("\t", string.Empty);

            // remove the leading new line characters
            while (s.StartsWith("\n"))
                s = s.Substring(1, s.Length - 1);

            // replace the remaining new lines
            s = s.Replace("\n", "<br/>");

            return s;
        }

        /// <summary>
        /// Disables the profile selector.
        /// </summary>
        private void DisableProfileSelector()
        {
            cboProfile.DropDown.Enabled = cboDataFormat.DropDown.Enabled = false;

            divProfileSelector.Style.Add(HtmlTextWriterStyle.BackgroundColor, "Red");
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>True or false, depending on whether it's valid or not</returns>
        private bool Validate()
        {
            var valid = this.cboExportType.SelectedItem.Value != "-";

            if (!valid)
                return valid;

            return this.cboExportType.SelectedItem.Value == "DXF"
                ? (valid && (this.cboDxfExport.SelectedItem.Value != "0" && !string.IsNullOrEmpty(this.cboDxfExport.SelectedItem.Value)))
                : valid 
                && this.cboProfile.SelectedItem.Value != "0"
                && !string.IsNullOrEmpty(this.cboProfile.SelectedItem.Value)
                && this.cboDataFormat.SelectedItem.Value != "0"
                && !string.IsNullOrEmpty(this.cboProfile.SelectedItem.Value);
        }

       #endregion

        #region Event Handlers

        /// <summary>
        /// Cboes the profile selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CboProfileSelectedIndexChanged(object sender, EventArgs e)
        {
            cboDataFormat.DropDown.Items.Clear();

            lblProfileDetail.Text = string.Empty;
            lblProfileDetail.Visible = false;

            if (cboProfile.DropDown.SelectedValue != "0" && !this.LoadCboDataFormat(cboProfile.DropDown.SelectedValue))
                this.DisableProfileSelector();

            if (cboProfile.DropDown.SelectedValue == "0")
            {
                this.SetForTDE();
            }
        }

        /// <summary>
        /// Cboes the export type selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CboExportTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblProfileDetail.Text = string.Empty;
            lblProfileDetail.Visible = false;

            this.cboDataFormat.DropDown.Items.Clear();

            switch (this.cboExportType.SelectedItem.Value)
            {
                case "DXF":
                    this.SetForDXF();
                    break;
                case "TDE":
                    this.SetForTDE();
                    break;
                default:
                    this.Reset();
                    this.DisableFields();
                    break;
            }
        }

        /// <summary>
        /// Cboes the DXF profile selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CboDxfProfileSelectedIndexChanged(object sender, EventArgs e)
        {
            var dxfProfileName = this.cboDxfExport.SelectedItem.Value;

            this.SetControlAttributes(false);

            if (dxfProfileName != "0")
            {
                var dxfProfile = DatashopService.Instance.JobService.GetDxfExportInfo(dxfProfileName);

                this.lblProfileDetail.Visible = true;
                this.lblProfileDetail.Text = this.CleanDescription(dxfProfile.Description);
            }
            else
                this.lblProfileDetail.Visible = false;
        }

       #endregion

    }
}