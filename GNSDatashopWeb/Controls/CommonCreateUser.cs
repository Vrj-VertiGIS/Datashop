namespace GEOCOM.GNSD.Web.Controls
{
    using GEOCOM.GNSD.Web.Config;

    public class CommonCreateUser : RequestUserControl
    {
        #region Public Properties

        public bool HasTitle { get; set; }

        public bool HasCancel { get; set; }

        /// <summary>
        /// Without any semi-colomn at the end, please.
        /// </summary>
        public string ClientCancelScript { get; set; }

        /// <summary>
        /// Give a chance to the client page to pass another config
        /// </summary>
        public PageFieldInfos FieldInfos { get; set; }

        #endregion

        #region Methods
        
        protected bool EnableBtnRequestBasedOnAgb(PageFieldInfos fieldInfos, Agb agb, Pds pds)
        {
            var agbVisible = IsFieldVisible(fieldInfos, "agb");
            var pdsVisible = IsFieldVisible(fieldInfos, "pds");
            var acceptedAll = (agb.Checked || !agbVisible) && (pds.Checked || !pdsVisible);
            return acceptedAll;
        }

        #endregion
    }
}
