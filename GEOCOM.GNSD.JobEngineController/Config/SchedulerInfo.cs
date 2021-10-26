using GEOCOM.GNSD.DBStore.DbAccess;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.JobEngineController.Config
{
	public class SchedulerInfo
	{
		// example for a simple tag
		[XmlElement("timer")]
		public TimerInfo Timer { get; set; }
		
		[XmlAttribute("maxjobprocesses")]
		public int MaxJobProcesses { get; set; }

        [XmlAttribute("cleaninginterval")]
        public int CleaningInterval { get; set; }

	    [XmlElement("loadbalancing")]
	    public LoadBalancingInfo LoadBalancing { get; set; }
	}

	public class TimerInfo
	{
		[XmlAttribute("scheduletimer")]
		public int ScheduleTimer { get; set; }
	}

	public class JobEngineInfo
	{
		[XmlAttribute("path")]
		public string Path { get; set; }
	}

    public class LoadBalancingInfo
    {
        [XmlAttribute("preference")]
        public LoadBalancingPreference Preference { get; set; }
    }

}