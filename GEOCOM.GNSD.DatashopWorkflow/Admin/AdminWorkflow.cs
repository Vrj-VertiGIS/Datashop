using System;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GEOCOM.GNSD.DatashopWorkflow.Admin
{
    public class AdminWorkflow : WorkflowBase<AdminWorkflowDataItem>
    {        
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, Process);
        }

        protected override void OnError(Exception e)
        {
            Utils.Utils.OnError(e, DataItem.JobId, GetType());
        }

        protected void Process()
        {
            AdminJobModel adminJobModel = DataItem.JobDescriptionModel as AdminJobModel;

            switch (adminJobModel.Action)
            {
                case AdminJobConst.UPDATEPLOTTEMPLATES:
                    PlotTemplateUpdater plotTemplateUpdater = new PlotTemplateUpdater();

                    // TODO MediumCode may be used later to distinguish between mediums -> Set to 0 at the moment                        
                    plotTemplateUpdater.UpdatePlotTemplatesInDb(DataItem.ExportConfig.PlotTemplate, 0);
                    break;
                default:
                    DatashopWorkflowDataItem.Logger.DebugFormat("Invalid action. Action={0} is not allowed.", adminJobModel.Action);
                    break;
            }
        }
    }
}
