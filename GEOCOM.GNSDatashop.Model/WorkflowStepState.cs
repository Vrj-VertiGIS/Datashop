using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
    public enum WorkflowStepState
    {
        [EnumMember]
        Idle = 0,

        [EnumMember]
        Running = 1,

        [EnumMember]
        Finished = 2,

        [EnumMember]
        Failed = 3,

		/// <summary>
		/// Used when the workflow is stopped by a StopAfterStep workflow interceptor.
		/// </summary>
		[EnumMember]
		StoppedAfterStep = 4
    }
}