using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSD.Workflow_Test
{
    public class WorkflowTracker
    {
        public IList<string> MethodsCalled { get; set; }
        private static WorkflowTracker _instance;

        public static WorkflowTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorkflowTracker();
                }
                return _instance;
            }
        }

    }
}
