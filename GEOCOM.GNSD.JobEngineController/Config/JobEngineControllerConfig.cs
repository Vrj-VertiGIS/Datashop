using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.JobEngineController.Config
{
    [XmlRoot("jobEngineController")]
    public class JobEngineControllerConfig : ConfigBase<JobEngineControllerConfig>
    {
        [XmlElement("scheduler")]
        public SchedulerInfo Scheduler { get; set; }

        [XmlElement("jobengine")]
        public JobEngineInfo JobEngine { get; set; }
    }
}