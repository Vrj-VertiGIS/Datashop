using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace GEOCOM.GNSDatashop.Model.Documents
{
    /// <summary>
    /// 
    /// </summary>
    [MessageContract]
    public class DocumentRequest
    {
        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        /// <value>
        /// The job id.
        /// </value>
        [MessageHeader]
        public long JobId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [MessageHeader]
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [MessageHeader]
        public string FileName { get; set; }

        /// <summary>
        /// Gets if the file exist on the system.
        /// </summary>
        [MessageHeader]
        public bool FileExists { get; set; }
    }

    /// <summary>
    /// For DatashopAddon - could not get the DocumentRequest generated over WSDL in Java
    /// </summary>
    [MessageContract]
    public class DocumentRequestDatashopAddon : DocumentRequest
    {
        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        /// <value>
        /// The job id.
        /// </value>
        [MessageBodyMember]
        public new long JobId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [MessageBodyMember]
        public new long UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [MessageBodyMember]
        public new string FileName { get; set; }
    }
}