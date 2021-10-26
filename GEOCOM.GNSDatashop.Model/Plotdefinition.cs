using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
	public class Plotdefinition
	{
        private PlotdefinitionKey _plotDefinitionKey = new PlotdefinitionKey();

        [DataMember]
        public virtual PlotdefinitionKey PlotdefinitionKey 
		{
			get { return _plotDefinitionKey; }
			set { _plotDefinitionKey = value; }
		}

        [DataMember]
		public virtual double? PlotHeightCm { get; set; }

        [DataMember]
        public virtual double? PlotWidthCm { get; set; }

        [DataMember]
        public virtual string Description { get; set; }

        [DataMember]
        public virtual string Roles { get; set; }

        [DataMember]
        public virtual string LimitsTimePeriods { get; set; }

        [DataMember]
        public virtual string Limits { get; set; }

        
	}
}