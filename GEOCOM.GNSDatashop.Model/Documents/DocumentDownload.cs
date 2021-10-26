using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GEOCOM.GNSDatashop.Model.Documents
{
    /// <summary>
    /// Class for transferring documents to and from the server
    /// </summary>
    [MessageContract]
    public class DocumentDownload : DocumentRequest
    {
        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        /// <value>
        /// The contents.
        /// </value>
        [MessageBodyMember]
        public Stream Contents { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        [MessageHeader]
        public long Length { get; set; }
    }
}