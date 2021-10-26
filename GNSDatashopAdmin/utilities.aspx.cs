using System;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GNSDatashopAdmin
{
    public partial class Utilities : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblUpdatePlotDefStatus.Text = "";
			lblJobProzStatus.Text = "";
			getAndBindData(true);
		}

		protected void UpdatePlotDefsClick(object sender, EventArgs e)
		{
			try
			{
                //Use factory
                AdminJobModel model = new AdminJobModel();
			    model.Action = AdminJobConst.UPDATEPLOTTEMPLATES;

                Job job = new Job();
                job.Definition = model.ToXml();
                job.ReasonId = 1;
                job.UserId = 1;  // TODO Always use Admin UserID                
                // job.ProcessorClassId = typeof(GEOCOM.GNSD.DatashopWorkflow.Admin.AdminWorkflow).FullName;
                job.ProcessorClassId = WorkflowDefinitions.AdminWorkflowClassId;

                // get a new webservice proxy to connect to the jobmanager 
                IJobManager jobMgrService = DatashopService.Instance.JobService;

                long jobID = jobMgrService.CreateJob(job);

                lblUpdatePlotDefStatus.Text = String.Format("Admin job to update the plottemplates created. JobId={0}.", jobID);
			}
			catch (Exception exp)
			{

                lblUpdatePlotDefStatus.Text = "Admin job to update the plottemplates couldn't be created: " + exp.Message.Split('\n')[0];
			}
		}



		protected void TemplateGridView_RowEditing(object sender, GridViewEditEventArgs e)
		{
			TemplateGridView.EditIndex = e.NewEditIndex;
			getAndBindData(false);

		}

		protected void TemplateGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			TemplateGridView.EditIndex = -1;
			getAndBindData(false);
		}

		protected void TemplateGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			var plot = GetPlotDefFromGridView(e.RowIndex);

			DatashopService.Instance.JobService.UpdateTemplate(plot);

			getAndBindData(false);
		}

		protected void TemplateGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			var plot = GetPlotDefFromGridView(e.RowIndex);

			DatashopService.Instance.JobService.DeleteTemplate(plot);

			getAndBindData(false);
		}

		protected void TemplateGridView_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			//every time an event is generated, it will stop any editing process that might be in action.
			TemplateGridView.EditIndex = -1;
			TemplateGridView.Columns[0].Visible = true;
		}

		private void getAndBindData(bool checkIfIsPageBack)
		{
			var templates = DatashopService.Instance.JobService.GetAllTemplates();
			TemplateGridView.DataSource = templates;

			if (checkIfIsPageBack)
			{
				if (!Page.IsPostBack)
				{
					TemplateGridView.DataBind();
				}
			}
			else
			{
				TemplateGridView.DataBind();
			}
		}

		private Plotdefinition GetPlotDefFromGridView(int rowIndex)
		{
			Plotdefinition plot = new Plotdefinition
			{
			    PlotdefinitionKey = new PlotdefinitionKey {Template = TemplateGridView.Rows[rowIndex].Cells[1].Text},
			    PlotHeightCm = double.Parse(TemplateGridView.Rows[rowIndex].Cells[2].Text),
			    PlotWidthCm = double.Parse(TemplateGridView.Rows[rowIndex].Cells[3].Text),
			    Description = ((TextBox) TemplateGridView.Rows[rowIndex].Cells[4].Controls[0]).Text,
			    Roles = ((TextBox) TemplateGridView.Rows[rowIndex].Cells[5].Controls[0]).Text,
			    LimitsTimePeriods = ((TextBox) TemplateGridView.Rows[rowIndex].Cells[6].Controls[0]).Text,
			    Limits = ((TextBox) TemplateGridView.Rows[rowIndex].Cells[7].Controls[0]).Text
			};

		    return plot;
		}
	}
}
