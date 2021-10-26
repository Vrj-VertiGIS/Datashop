using System.IO;
using System.ServiceModel;

namespace GEOCOM.GNSDatashop.Model.Documents
{
    /// <summary>
    /// 
    /// </summary>
    [MessageContract]
    public class DocumentUpload : DocumentRequest
    {
        [MessageBodyMember]
        public Stream Contents { get; set; }

        [MessageHeader]
        public long Length { get; set; }
    }
}
