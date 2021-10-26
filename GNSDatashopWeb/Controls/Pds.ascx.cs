using System;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.ServerControls;

namespace GEOCOM.GNSD.Web.Controls
{

    /// <summary>
    /// PDS steht für Privatdatenschutz :-)
    /// Wia kopierte das AGB UserControl
    /// </summary>
    public partial class Pds : ValidatedControl
    {
        #region Private variables
        private IMsg _log;
        #endregion

        public Pds()
        {
            this.InitLogger();
        }

        #region Public events

        public event EventHandler CheckedChanged
        {
            add
            {
                chkAcceptPDS.CheckedChanged += value;
            }
            
            remove
            {
                chkAcceptPDS.CheckedChanged -= value;
            }
        }
        #endregion

        #region Public properties

        public string OnClientClick { get; set; }

        public bool AutoPostBack
        {
            get
            {
                return chkAcceptPDS.AutoPostBack;
            }
            
            set
            {
                chkAcceptPDS.AutoPostBack = value;
            }
        }

        public bool Checked
        {
            get
            {
                return chkAcceptPDS.Checked;
            }
            
            set
            {
                chkAcceptPDS.Checked = value;
            }
        }

        public override bool Required
        {
            get
            {
                return true;
            }
            
            set
            {
            }
        }
        #endregion

// ReSharper disable InconsistentNaming
        protected void Page_Load(object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            // intentionally left blank
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            chkAcceptPDS.InputAttributes.Add("OnClick", this.OnClientClick);
        }

        protected override void OnValidate(ValidateEventArgs e)
        {
            if (!chkAcceptPDS.Checked)
            {
                e.IsValid = false;
                e.Message = WebLanguage.LoadStr(2038, "Please accept the private data protection clause");
            }
        }

        private void InitLogger()
        {
            try
            {
                if (this._log == null)
                {
                    DatashopLogInitializer.Initialize();
                    this._log = new Msg(GetType());
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }
    }
}