using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
    public class PlacementOption
    {
        [DataMember]
        public virtual long PlacementOptionId { get; set; }

        [DataMember]
        public virtual string Text { get; set; }
    }
}