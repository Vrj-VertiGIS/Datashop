using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
	/// <summary>
	/// Definition of plot output related parameters
	/// for a given plot extent - i.e. a4 landscape
	/// </summary>
	[DataContract]
	public class PlotFormatDef 
	{
        public PlotFormatDef()
        {            
        }

        public PlotFormatDef( string template, double? plotHeight, double? plotWidth, string description, int remainingLimit)
        {
            Template = template;
            PlotHeightCM = plotHeight ?? 0.0;
            PlotWidthCM = plotWidth ?? 0.0;
            Description = description;
            RemainingLimit = remainingLimit;
        }

        [DataMember]
		public string Template { get; set; }

        [DataMember]
		public double PlotHeightCM { get; set; }

        [DataMember]
		public double PlotWidthCM { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Roles { get; set; }

        [DataMember]
        public int RemainingLimit { get; set; }
	}
}
