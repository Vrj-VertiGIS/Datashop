using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
	public class RestrictionInfo
	{
		private string _maxPlotsRate;

		/// <summary>
		/// This was a mistakenly named property and is replaced by <see cref="MaxPlotsRateInternal"/>. 
		/// Just for backwards compatibility
		/// </summary>
		[XmlAttribute("maxJobsRate")] 
		[Obsolete]
		public string MaxJobsRateInternal
		{
			get { return MaxPlotsRateInternal; }
			set { MaxPlotsRateInternal = value; }
		}


		[XmlAttribute("maxPlotsRate")]
		public string MaxPlotsRateInternal
		{
			get { return _maxPlotsRate; }
			set { _maxPlotsRate = value; }
		}

		
		public int MaxPlotsRate
		{
			get
			{
				int maxJobsRate;
				const int minimalRate = 1;
				if (int.TryParse(MaxPlotsRateInternal, out maxJobsRate) || maxJobsRate > minimalRate)
				{
					return maxJobsRate;
				}
				else
				{
					const int defaultMinimalRate = 20;
					return defaultMinimalRate;
				}
			}
		
		}

		[XmlElement("restriction")]
		public Restriction[] Restrictions { get; set; }
	}

	public class Restriction
	{
		[XmlAttribute("role")]
		public string Role { get; set; }

		[XmlAttribute("timeperiod")]
		public int TimePeriod { get; set; }

		[XmlAttribute("limit")]
		public int Limit { get; set; }
	}
}
