using System.Collections.Generic;
using System.Runtime.Serialization;
using GEOCOM.GNSD.Common.Model;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
    public class ProfileInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<OutputFormat> TargetFormat { get; set; }

        [DataMember]
        public List<string> PostProcessList { get; set; }

        [DataMember]
        public string Guid { get; set; }
    }
}
