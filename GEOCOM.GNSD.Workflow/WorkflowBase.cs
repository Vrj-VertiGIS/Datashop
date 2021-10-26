using GEOCOM.GNSD.Workflow.Attributes;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSD.Workflow.Exceptions;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GEOCOM.GNSD.Workflow
{
    public abstract class WorkflowBase : IWorkflow
    {
        private WorkflowStateItem _stateItem;
        private IWorkflowStepInterceptor _stepInterceptor;

        public WorkflowStepState CurrentWorkflowStepState { get; set; }

        public bool IsActive { get; set; }

        public bool NeedsProcessing { get; set; }

        public long ProcessId { get; set; }

        private LinkedListNode<WorkflowStep> CurrentWorkflowStep { get; set; }

        private LinkedList<WorkflowStep> WorkflowSteps { get; set; }

        protected WorkflowDataItemBase DataItem { get; set; }

        protected WorkflowBase()
        {
            WorkflowSteps = new LinkedList<WorkflowStep>();
            WorkflowSteps.AddLast(new WorkflowStep(0, Start, null, null));
            var workflowDefinition = new WorkflowDefinition(WorkflowSteps);
            DefineWorkflow(workflowDefinition);

            CheckWorkflow();

            CurrentWorkflowStep = WorkflowSteps.First;
            CurrentWorkflowStepState = WorkflowStepState.Idle;
            IsActive = true;
        }

        public static WorkflowBase CreateWorkflowFromStateItem(WorkflowStateItem stateItem, WorkflowDataItemBase dataItem, bool doDataBinding)
        {
            try
            {
                var workflowBase = Activator.CreateInstance(stateItem.AssemblyFullName, stateItem.ClassId).Unwrap() as WorkflowBase;
                if (workflowBase == null)
                {
                    throw new WorkflowException("Unable to create a WorkflowBase class from the provided class name " +
                                                stateItem.ClassId);
                }

                workflowBase.IsActive = stateItem.IsActive;
                workflowBase.NeedsProcessing = stateItem.NeedsProcessing;
                workflowBase.ProcessId = stateItem.ProcessId;
                workflowBase.CurrentWorkflowStep = workflowBase.GetWorkflowStepById(stateItem.WorkflowStepId);
                workflowBase.CurrentWorkflowStepState = stateItem.CurrentWorkflowStepState;

                if (doDataBinding)
                {
                    workflowBase.DataBind(dataItem);
                }
                workflowBase._stateItem = stateItem;
                return workflowBase;
            }
            catch (Exception e)
            {
                throw new Exception($"Could not create the workflow for job id={stateItem.Id}: '{e.Message}'.", e);
            }
        }

        #region IWorkflow Members

        public void Activate()
        {
            if (IsActive)
                return;

            bool workflowStoppedOnBarrier = CurrentWorkflowStep.Value == Barrier;
            bool workflowInterceptedAndStopedAfterStep = CurrentWorkflowStepState == WorkflowStepState.StoppedAfterStep;
            if (workflowStoppedOnBarrier || workflowInterceptedAndStopedAfterStep)
                PrepareNextStep();

            IsActive = true;
            NeedsProcessing = true;
            ProcessId = 0;
            CurrentWorkflowStepState = WorkflowStepState.Idle;

            UpdateDataItem();
        }

        public void SetFailed()
        {
            IsActive = false;
            CurrentWorkflowStepState = WorkflowStepState.Failed;
            UpdateDataItem();
        }

        public void Run()
        {
            WorkflowStep workflowStep = CurrentWorkflowStep.Value;
            if (workflowStep == null || !IsActive)
                return;

            try
            {
                bool processStep = workflowStep.SkipStepCondition == null || !workflowStep.SkipStepCondition();
                if (processStep)
                {
                    CurrentWorkflowStepState = WorkflowStepState.Running;
                    UpdateDataItem();

                    workflowStep.Function();

                    CurrentWorkflowStepState = WorkflowStepState.Finished;

                    bool reachedEndOfWorkflow = CurrentWorkflowStep == WorkflowSteps.Last;
                    var stopAfterThisStep = ShouldStopAfterStep(workflowStep);
                    if (reachedEndOfWorkflow || stopAfterThisStep)
                        IsActive = false;

                    if (stopAfterThisStep)
                        OnStoppedAfterStep(workflowStep);

                    if (stopAfterThisStep)
                        CurrentWorkflowStepState = WorkflowStepState.StoppedAfterStep;

                    UpdateDataItem();
                }
            }
            catch (Exception e)
            {
                // Always breaks in case of exceptions during a debug session.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                CurrentWorkflowStepState = WorkflowStepState.Failed;
                IsActive = false;
                UpdateDataItem();
                OnError(e);
                throw;
            }

            if (IsActive)
            {
                PrepareNextStep();
                Run();
            }
        }

        #endregion

        public string GetWorkflowStepNameById(int workflowStepId)
        {
            ValidateWorkflowStepId(workflowStepId);
            LinkedListNode<WorkflowStep> workflowStep = GetWorkflowStepById(workflowStepId);
            return GetWorkflowStepNameByWorkflowStep(workflowStep.Value);
        }

        public int[] GetAllRestartableStepIds()
        {
            List<int> restartableStepsIndices = new List<int>();
            foreach (WorkflowStep workflowStep in WorkflowSteps)
            {
                restartableStepsIndices.Add(workflowStep.StepId);
                if (workflowStep.StepId == CurrentWorkflowStep.Value.StepId)
                {
                    break;
                }
            }
            return restartableStepsIndices.ToArray();
        }

        public WorkflowStepIdName[] GetAllStepIdNames()
        {
            return GetWorkflowStepIdNames(WorkflowSteps);
        }

        public WorkflowStepIdName[] GetAllRestartableStepIdNames()
        {
            LinkedList<WorkflowStep> restartableWorkflowSteps = GetAllRestartableSteps();
            return GetWorkflowStepIdNames(restartableWorkflowSteps);
        }

        public WorkflowStepIdName GetNextStepIdName()
        {
            LinkedListNode<WorkflowStep> nextStep = CurrentWorkflowStep.Next;
            if (nextStep != null)
            {
                string stepName = GetWorkflowStepNameByWorkflowStep(nextStep.Value);
                var indexName = new WorkflowStepIdName { Id = nextStep.Value.StepId, Name = stepName };
                return indexName;
            }
            throw new WorkflowException("End step reached. No next step available!");
        }

        public void RestartFromStep(int workflowStepId)
        {
            ValidateWorkflowStepId(workflowStepId);

            CurrentWorkflowStep = GetWorkflowStepById(workflowStepId);

            CurrentWorkflowStepState = WorkflowStepState.Idle;
            if (CurrentWorkflowStep.Value == Barrier)
            {
                IsActive = false;
            }
            else
            {
                IsActive = true;
                NeedsProcessing = true;
            }
            UpdateDataItem();
        }

        protected virtual void DataBind(WorkflowDataItemBase dataItemBase)
        {
        }

        /// <summary>
        /// If overridden, creates an instance of the <see cref="IWorkflowStepInterceptor"/> interface 
        /// that is used to influence workflow processing.  
        /// </summary>
        /// <returns></returns>
        protected virtual IWorkflowStepInterceptor GetWorkflowInterceptor()
        {
            return null;
        }

        /// <summary>
        /// <remarks>This virtual method is called from the WorkflowBase constructor which may be a potencional problem because it is called before the derived class constructor is.</remarks>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms182331(v=VS.100).aspx"/>
        /// <param name="workflowDefinition"></param>
        /// </summary>
        protected abstract void DefineWorkflow(IWorkflowDefinition workflowDefinition);

        protected void Barrier()
        {
            IsActive = false;
        }

        protected virtual void OnError(Exception exception)
        {

        }

        protected virtual void OnStoppedAfterStep(IWorkflowStep workflowStep)
        {

        }

        private void Start()
        {
        }

        private void CheckWorkflow()
        {
            if (WorkflowSteps.Count == 1)
            {
                throw new WorkflowException("Invalid workflow. Must not be empty.");
            }
            List<int> stepIds = new List<int>();
            foreach (WorkflowStep workflowStep in WorkflowSteps)
            {
                if (stepIds.Contains(workflowStep.StepId))
                {
                    throw new WorkflowException(
                        "Invalid workflow. StepId has to be unique! StepId=0 is reserved for Start. StepId=int.MaxValue is reserverd for End.");
                }
                stepIds.Add(workflowStep.StepId);
            }
        }

        private void UpdateDataItem()
        {
            _stateItem.IsActive = IsActive;
            _stateItem.NeedsProcessing = NeedsProcessing;
            _stateItem.WorkflowStepId = CurrentWorkflowStep.Value.StepId;
            _stateItem.CurrentWorkflowStepState = CurrentWorkflowStepState;
            _stateItem.ProcessId = ProcessId;
            _stateItem.Update();
        }

        private LinkedList<WorkflowStep> GetAllRestartableSteps()
        {
            LinkedList<WorkflowStep> restartableSteps = new LinkedList<WorkflowStep>();
            foreach (WorkflowStep workflowStep in WorkflowSteps)
            {
                restartableSteps.AddLast(workflowStep);
                if (workflowStep.StepId == CurrentWorkflowStep.Value.StepId)
                {
                    break;
                }
            }
            return restartableSteps;
        }

        private WorkflowStepIdName[] GetWorkflowStepIdNames(LinkedList<WorkflowStep> workflowSteps)
        {
            List<WorkflowStepIdName> nameIndexList = new List<WorkflowStepIdName>();
            foreach (WorkflowStep workflowStep in workflowSteps)
            {
                string stepName = GetWorkflowStepNameById(workflowStep.StepId);
                var indexName = new WorkflowStepIdName { Id = workflowStep.StepId, Name = stepName };
                nameIndexList.Add(indexName);
            }
            return nameIndexList.ToArray();
        }

        private string GetWorkflowStepNameByWorkflowStep(WorkflowStep workflowStep)
        {

            var workflowStepNameAttribute =
                workflowStep.Function.Method.GetCustomAttributes(typeof(WorkflowStepNameAttribute), false)
                .SingleOrDefault() as WorkflowStepNameAttribute;
            if (workflowStepNameAttribute != null && !string.IsNullOrEmpty(workflowStepNameAttribute.Name))
            {
                return workflowStepNameAttribute.Name;
            }

            return workflowStep.Name; ;
        }

        private LinkedListNode<WorkflowStep> GetWorkflowStepById(int workflowStepId)
        {
            LinkedListNode<WorkflowStep> currentLoopStep = WorkflowSteps.First;
            for (int i = 0; i < WorkflowSteps.Count; i++)
            {
                if (currentLoopStep.Value.StepId == workflowStepId)
                {
                    return currentLoopStep;
                }
                currentLoopStep = currentLoopStep.Next;
            }
            throw new WorkflowException($"Invalid workflow step id={workflowStepId}.");
        }

        private void ValidateWorkflowStepId(int workflowStepId)
        {
            bool containsStepId = WorkflowSteps.Any(workflowStep => workflowStep.StepId == workflowStepId);
            if (!containsStepId)
                throw new WorkflowException($"Invalid workflow step id={workflowStepId}.");
        }

        private void PrepareNextStep()
        {
            CurrentWorkflowStep = CurrentWorkflowStep.Next;
        }

        private bool ShouldStopAfterStep(WorkflowStep workflowStep)
        {
            _stepInterceptor = _stepInterceptor ?? GetWorkflowInterceptor();
            bool stopAfterThisStep = _stepInterceptor != null && _stepInterceptor.StopAfterStep(workflowStep, DataItem);

            return stopAfterThisStep;
        }
    }

    public abstract class WorkflowBase<TDataItem> : WorkflowBase where TDataItem : WorkflowDataItemBase, new()
    {
        protected new TDataItem DataItem
        {
            get { return base.DataItem as TDataItem; }
            set { base.DataItem = value; }
        }

        protected sealed override void DataBind(WorkflowDataItemBase dataItem)
        {
            DataItem = new TDataItem();
            DataItem.Init(dataItem);
        }
    }
}