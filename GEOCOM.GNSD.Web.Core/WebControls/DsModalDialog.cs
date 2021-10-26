using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web.Core.WebControls
{
    /// <summary>
    /// wia come back
    /// </summary>
    public delegate void ModalDialogNotify();

    /// <summary>
    /// This implements a modal dialog panel whith templated content (element DialogTemplate).
    /// Optionaly, the content can be encapsulated by an UpdatePanel (see the IsUpdatePanel property).
    /// The client Logic is defined in the javascript file ModalDialog.js.
    /// The client logic is performed by the object DSMD[ctlId]
    /// </summary>
    public class DsModalDialog : WebControl, INamingContainer
    {
        private const string CtlPrefix = "DSMD";
        private const string CssPrefix = "DSMD";

        #region Private Member Variables

        // these are all the controls that are always rendered
        private Panel divDialogBox = new Panel();
        private Panel divDialogTitle = new Panel();
        private Label lblTitle = new Label();
        private Panel divDialogBody = new Panel();
        private Panel divDialogBackground = new Panel();
        
        // the updatePanel control is loaded when IsUpdatePanel is true
        private UpdatePanel updatePanel;

        // these are the objects required by ITemplate
        private DialogData dialogData = null;
        private ITemplate dialogTemplate = null;
        
        // when the control is displayed or repositioned, we get its position (left and top) via the hiddenState field
        private HiddenField hiddenState = new HiddenField();
        #endregion

        public DsModalDialog()
        {
            this.Load += new EventHandler(this.ModalDialogLoad);
            this.PreRender += new EventHandler(this.ModalDialogPreRender);
        }

        public event ModalDialogNotify DialogLoaded;

        #region Properties
        
        public string Title
        {
            get { return this.lblTitle.Text; }
            set { this.lblTitle.Text = value; }
        }

        /// <summary>
        /// to support nested modal dialogs
        /// </summary>
        public int ZIndex { get; set; }

        public int OffsetTop { get; set; }
        public int OffsetLeft { get; set; }

        /// <summary>
        /// will add an update panel around the content
        /// </summary>
        public bool IsUpdatePanel { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DialogData DialogData
        {
            get
            {
                this.EnsureChildControls();
                return this.dialogData;
            }
        }

        [Browsable(false), DefaultValue(null), Description("The dialog template."), TemplateContainer(typeof(DialogData)),
        PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate DialogTemplate
        {
            get
            {
                return dialogTemplate;
            }
            set
            {
                dialogTemplate = value;
            }
        }

        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        #endregion

        #region Methods
        public void Show()
        {
            this.hiddenState.Value = "0;0";
        }

        public void Hide()
        {
            this.hiddenState.Value = string.Empty;
            //////if (this.IsUpdatePanel)
            //////{
            //////    ScriptManager.RegisterStartupScript(
            //////        this, typeof(Page), "HideMd", string.Format("{1}.Hide();", CtlPrefix, ID), true);
            //////}
        }
        #endregion

        #region Internal Design Helper Methods
        // This internal helper method is used by the designer to set the StatsData instance
        // to a specified StatsData.  Note that the public StatsData property is read-only...
        internal void AddDialogData(DialogData dd)
        {
            this.dialogData = dd;
        }
        #endregion

        #region Overridden Control Members
        protected override void CreateChildControls()
        {

            // get the template
            ITemplate template = this.DialogTemplate;
            if (template == null)
            {
                return;
            }

            // add the StatsData to the control hierarchy
            if (this.IsUpdatePanel)
            {
                this.updatePanel = new UpdatePanel();
                this.updatePanel.ID = "upd" + ID;
                this.updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                this.updatePanel.ChildrenAsTriggers = true;
                template.InstantiateIn(this.updatePanel.ContentTemplateContainer);
                this.divDialogBody.Controls.Add(this.updatePanel);
            }
            else
            {
                // create a new StatsData instance based on the property values
                this.dialogData = new DialogData();

                // instantiate the StatsData in the template
                template.InstantiateIn(this.dialogData);
                this.divDialogBody.Controls.Add(this.dialogData);
            }
        }
        
        /// <summary>
        ///  do not render the embedding span
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            RenderChildren(output);
        }

        #endregion

        protected void ModalDialogLoad(object sender, EventArgs e)
        {
            // dialog state (visible/hidden, left, top)
            this.Controls.Add(this.hiddenState);
            
            // dialog background
            this.divDialogBackground.CssClass = CssPrefix + "Background";
            this.divDialogBackground.Style.Add("z-index", this.ZIndex.ToString());
            this.Controls.Add(this.divDialogBackground);
            
            // dialog box - the container for everything that comes on top of the background
            //// this.divDialogBox.Height = Height;
            this.divDialogBox.Style.Add("min-height", Height.ToString());
            this.divDialogBox.Width = Width;
            this.divDialogBox.CssClass = CssPrefix + "Box";
            this.divDialogBox.Style.Add("z-index", (this.ZIndex + 1).ToString());
            this.Controls.Add(this.divDialogBox);
            
            // title bar
            this.divDialogTitle.CssClass = CssPrefix + "Title";
            this.divDialogTitle.Controls.Add(this.lblTitle);
            this.divDialogBox.Controls.Add(this.divDialogTitle);
            
            // dialog body
            this.divDialogBody.CssClass = CssPrefix + "Body";
            this.divDialogBox.Controls.Add(this.divDialogBody);

            // notify the parent control that the controls in the template are available
            if (this.DialogLoaded != null)
            {
                this.DialogLoaded();
            }
        }

        protected void ModalDialogPreRender(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            // show/hide the dialog box
            var hidden = string.IsNullOrEmpty(this.hiddenState.Value);
            if (hidden)
            {
                this.divDialogBackground.Style.Add("display", "none");
                this.divDialogBox.Style.Add("display", "none");
                this.divDialogBox.Style.Remove("left");
                this.divDialogBox.Style.Remove("top");
            }
            else {
                this.divDialogBackground.Style.Add("display", string.Empty);
                this.divDialogBox.Style.Add("display", string.Empty);
            }

            // generate the client scripts
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), CtlPrefix + "Include", ResolveClientUrl("~/js/DsModalDialog.js"));
            var initScript = string.Format("var {1} = new {0}('{2}', '{3}', '{4}', {5}, {6});\nAddLoadScript({1}.Init);\n",
                CtlPrefix, ID, divDialogBox.ClientID, divDialogBackground.ClientID, hiddenState.ClientID, OffsetLeft, OffsetTop);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), CtlPrefix + "Init" + ID, initScript, true);
        }
    }

    #region Dialog Template Container

    /// <summary>
    /// This is a dummy container - no data available for binding
    /// </summary>
    [ToolboxItem(false)]
    public class DialogData : WebControl, INamingContainer
    {

        /// <summary>
        ///  do not render the embedding span
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            RenderChildren(output);
        }
    }

    #endregion

}
