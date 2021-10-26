using System;

namespace GEOCOM.GNSD.Workflow.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class WorkflowStepNameAttribute : Attribute
    {
        public string Name { get; set; }    
        public WorkflowStepNameAttribute(string name)
        {
            Name = name;
        }

        
    }
}
