using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
    public class PlotSection
    {
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string Description { get; set; }

        [DataMember]
        public virtual string VisibleGroupLayers { get; set; }
    }
}
