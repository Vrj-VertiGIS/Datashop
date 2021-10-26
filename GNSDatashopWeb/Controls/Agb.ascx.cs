using System;
using System.ComponentModel;
using System.IO;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.ServerControls;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// User control that presents a checkbox to accept agb's and throws the event CheckChanged when this happens
    /// </summary>
    public partial class Agb : ValidatedControl
    {
        private IMsg _log;

        public Agb()
        {
            InitLogger();
        }

        [Browsable(true)]
        public event EventHandler CheckedChanged
        {
            add
            {
                chkAcceptAGB.CheckedChanged += value;
            }

            remove
            {
                chkAcceptAGB.CheckedChanged -= value;
            }
        }

        public string OnClientClick { get; set; }

        public bool AutoPostBack
        {
            get
            {
                return chkAcceptAGB.AutoPostBack;
            }

            set
            {
                chkAcceptAGB.AutoPostBack = value;
            }
        }

        public bool Checked
        {
            get
            {
                return chkAcceptAGB.Checked;
            }

            set
            {
                chkAcceptAGB.Checked = value;
            }
        }

        public bool ForceTempUserAGB { get; set; }

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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetAgbUrl();
            chkAcceptAGB.InputAttributes.Add("OnClick", OnClientClick);
        }

        protected override void OnValidate(ValidateEventArgs e)
        {
            if (!chkAcceptAGB.Checked)
            {
                e.IsValid = false;
                e.Message = WebLanguage.LoadStr(2033, "Please accept the general terms and conditions");
            }
        }

        private void InitLogger()
        {
            try
            {
                if (_log == null)
                {
                    DatashopLogInitializer.Initialize();
                    _log = new Msg(GetType());
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }

        private void SetAgbUrl()
        {
            string agbUrl;
            var tempUserAGB = DatashopWebConfig.Instance.GetWebDocumentRelaviveUrlByName("agbTempUser");
            if ((Page.User.IsInRole("TEMP") || ForceTempUserAGB) && tempUserAGB != null)
            {
                agbUrl = Page.ResolveUrl(tempUserAGB);
            }
            else
            {
                agbUrl = Page.ResolveUrl(DatashopWebConfig.Instance.GetWebDocumentRelaviveUrlByName("agb"));
            }
            string agbPath = Server.MapPath(agbUrl);
            if (!File.Exists(agbPath))
            {
                _log.Fatal("AGB file was not found");
                throw new Exception(string.Format("The AGB File {0} is missing", agbPath));
            }

            linkAgb.NavigateUrl = agbUrl;
        }
    }
}