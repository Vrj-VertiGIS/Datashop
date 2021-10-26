using System.Collections.Generic;
using System.Diagnostics;

namespace GNSDatashopTest.Workflow
{
    public class WorkflowTracker
    {
        public IList<string> CalledMethods { get; set; }
        private static WorkflowTracker _instance;

		/// <summary>
		/// Add calling method to the CalledMethods collection.
		/// </summary>
		public void TrackThisMethod()
		{
			var stackTrace = new StackTrace(1);
			CalledMethods.Add(stackTrace.GetFrame(0).GetMethod().Name);
		}

        public static WorkflowTracker Instance
        {
            get { return _instance ?? (_instance = new WorkflowTracker()); }
        }

    }
}
