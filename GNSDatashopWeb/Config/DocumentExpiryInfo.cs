using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    /// <summary>
    /// Class that corresponds to the configuration for the Datashop Web
    /// </summary>
    public class DocumentExpiryInfo
    {
        #region Fields

        /// <summary>
        /// The default archive days
        /// </summary>
        private const int DefaultArchiveDays = 30;

        /// <summary>
        /// The default full expiration days
        /// </summary>
        private const int DefaultFullExpirationDays = 30;

        /// <summary>
        /// The archive after days
        /// </summary>
        private int _archiveAfterDays;

        /// <summary>
        /// The restart job expiration
        /// </summary>
        private int _restartJobExpiration;

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DocumentExpiryInfo"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the archive after days.
        /// </summary>
        /// <value>
        /// The archive after days.
        /// </value>
        [XmlAttribute("expireafterdays")]
        public int ArchiveAfterDays
        {
            get { return _archiveAfterDays; }
            set { _archiveAfterDays = value <= 0 ? DefaultArchiveDays : value; }
        }

        /// <summary>
        /// Gets or sets the restart job expiration.
        /// </summary>
        /// <value>
        /// The restart job expiration.
        /// </value>
        [XmlAttribute("restartjobexpiration")]
        public int RestartJobExpiration
        {
            get { return _restartJobExpiration; }
            set { _restartJobExpiration = value <= 0 ? DefaultFullExpirationDays : value; }
        }
    }
}