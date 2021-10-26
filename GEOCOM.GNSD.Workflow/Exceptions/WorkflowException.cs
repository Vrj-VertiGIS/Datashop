using System;

namespace GEOCOM.GNSD.Workflow.Exceptions
{
    public class WorkflowException : Exception
    {
        public WorkflowException()
        {
        }

        public WorkflowException(string msg)
            : base(msg)
        {
        }
    }
}
