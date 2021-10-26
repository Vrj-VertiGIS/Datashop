using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.Web.Config
{

    /// <summary>
    /// Config class for the Datashop Web
    /// </summary>
    [XmlRoot("datashopWeb")]
    public class DatashopWebConfig : ConfigBase<DatashopWebConfig>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the representative job.
        /// </summary>
        /// <value>
        /// The representative job.
        /// </value>
        [XmlElement("representativejob")]
        public RepresentativeJobInfo RepresentativeJob { get; set; }

        /// <summary>
        /// Gets or sets the document expiry.
        /// </summary>
        /// <value>
        /// The document expiry.
        /// </value>
        [XmlElement("documentexpiry")]
        public DocumentExpiryInfo DocumentExpiry { get; set; }

        /// <summary>
        /// Gets or sets the surrogate request.
        /// </summary>
        /// <value>
        /// The surrogate request.
        /// </value>
        [XmlElement("surrogaterequest")]
        public SurrogateRequestInfo SurrogateRequest { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        [XmlElement("languages")]
        public LanguageFileInfos Languages { get; set; }

        /// <summary>
        /// If set to true, the detailed exception messages are shown also for asynchronous requests.
        /// </summary>
        [XmlAttribute("debug")]
        public string Debug { get; set; }

        /// <summary>
        /// Gets or sets the map service.
        /// </summary>
        /// <value>
        /// The map service.
        /// </value>
        [XmlElement("mapservice")]
        public MapServiceInfo MapService { get; set; }

		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>
		/// The proxy.
		/// </value>
		[XmlElement("proxy")]
		public ProxyConfig Proxy { get; set; }

        /// <summary>
        /// Gets or sets the web documents.
        /// </summary>
        /// <value>
        /// The web documents.
        /// </value>
        [XmlElement("webdocuments")]
        public WebDocumentsInfo WebDocuments { get; set; }

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        /// <value>
        /// The security.
        /// </value>
        [XmlElement("security")]
        public Security Security { get; set; }

        /// <summary>
        /// Gets or sets the request page config.
        /// </summary>
        /// <value>
        /// The plot request page config.
        /// </value>
        [XmlElement("requestPage")]
        public RequestPageConfig RequestPageConfig { get; set; }

        /// <summary>
        /// Gets or sets the register business user page field infos.
        /// </summary>
        /// <value>
        /// The register business user page field infos.
        /// </value>
        [XmlElement("registerBusinessUserPage")]
        public PageFieldInfos RegisterBusinessUserPageFieldInfos { get; set; }

        /// <summary>
        /// Gets or sets the login temp user page field infos.
        /// </summary>
        /// <value>
        /// The login temp user page field infos.
        /// </value>
        [XmlElement("loginTempUserPage")]
        public PageFieldInfos LoginTempUserPageFieldInfos { get; set; }

        /// <summary>
        /// Gets or sets the online help info.
        /// </summary>
        /// <value>
        /// The online help info.
        /// </value>
        [XmlElement("onlineHelp")]
        public OnlineHelpInfo OnlineHelpInfo { get; set; }

        /// <summary>
        /// Gets or sets the geo-attachments configuration.
        /// </summary>
        /// <value>
        /// The geo attachments configuration.
        /// </value>
        [XmlElement("geoattachments")]
        public GeoAttachemntsConfig GeoAttachments { get; set; }

        /// <summary>
        /// Gets or sets the map search configuraiton.
        /// </summary>
        /// <value>
        /// The map search configurations.
        /// </value>
        [XmlElement("mapSearch")]
        public MapSearchConfig MapSearch{ get; set; }

        public RequestPageInfo DefaultRequestPage
        {
            get
            {
           
                    return new RequestPageInfo("RequestPage.aspx", 3004, true);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the document path.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetDocumentPath(string name)
        {
            var document = this.WebDocuments.Documents.Where(d => d.Name == name)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            return document == null ? null : document.RelativeUrl;
        }

        /// <summary>
        /// Gets the field info by id.
        /// </summary>
        /// <param name="fieldInfos">The field infos.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PageFieldInfo GetFieldInfoById(PageFieldInfos fieldInfos, string id)
        {
            return fieldInfos == null ? null : fieldInfos.GetById(id);
        }

      
        /// <summary>
        /// Gets the register business user page field info by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PageFieldInfo GetRegisterBusinessUserPageFieldInfoById(string id)
        {
            return this.RegisterBusinessUserPageFieldInfos.GetById(id);
        }

        /// <summary>
        /// Gets the login temp user page field infos.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PageFieldInfo GetLoginTempUserPageFieldInfos(string id)
        {
            return this.LoginTempUserPageFieldInfos.GetById(id);
        }

        /// <summary>
        /// Gets the name of the web document relavive URL by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetWebDocumentRelaviveUrlByName(string name)
        {
            return this.WebDocuments.Documents.Where(d => d.Name == name)
                .Select(d => d.RelativeUrl)
                .SingleOrDefault();
        }

        #endregion
    }
}
