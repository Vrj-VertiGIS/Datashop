using System;
using System.Collections.Generic;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSD.Workflow.Delegates;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GEOCOM.GNSD.Workflow
{
	class WorkflowDefinition : IWorkflowDefinition
	{
		private readonly LinkedList<WorkflowStep> _linkedList;

		public WorkflowDefinition(LinkedList<WorkflowStep> linkedList)
		{
			_linkedList = linkedList;
		}

		public void AddLast(int stepId, WorkflowFunction function)
		{
			AddLast(stepId, function, null, null);
		}

		public void AddLast(int stepId, WorkflowFunction function, string name)
		{
			AddLast(stepId, function, null, name);
		}

		public void AddLast(int stepId, WorkflowFunction function, WorkflowCondition skipStepCondition)
		{
			AddLast(stepId, function, skipStepCondition, null);
		}

		public void AddLast(int stepId, WorkflowFunction function, WorkflowCondition skipStepCondition, string name)
		{
			_linkedList.AddLast(new WorkflowStep(stepId, function, skipStepCondition, name));
		}
	}
}
