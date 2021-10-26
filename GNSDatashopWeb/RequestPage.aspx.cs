using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using GEOCOM.GNSD.Common.JobFactory;
using GEOCOM.GNSD.Common.JobFactory2;
using GEOCOM.GNSD.Common.Model;
using System.Linq;
using System.ServiceModel;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Controls;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSD.Web.JavascriptSerializer;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSD.Web
{
	/// <summary>
	/// Code behind class for the Plot Request Page
	/// </summary>
	public partial class RequestPage : Controls.RequestPage
	{
		protected static RequestPageMode RequestPageMode { get { return DatashopWebConfig.Instance.RequestPageConfig.ActiveMode; } }

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RequestPage"/> class.
		/// </summary>
		public RequestPage() : base(DatashopWebConfig.Instance.RequestPageConfig.PageFieldInfos) { }

		#endregion

		#region Page Lifecycle Events

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				InitPage();
				if (RequestPageMode == RequestPageMode.Data)
				{
					DatashopWebConfig.Instance.RequestPageConfig.DataMode.CheckIt();
				}
			}
			catch (Exception ex)
			{
				LogError("RequestPage initialization failed. Reason:", ex);
				Response.RedirectSafe("error/GeneralErrorPage.aspx", false);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			CheckUser();

			if (!Page.IsPostBack)
			{
				BindPlotSections();

				ctlActAsSurrogate.Visible = User.IsInRole("ADMIN") || User.IsInRole("REPRESENTATIVE");

			    var dxfConfig = DatashopWebConfig.Instance.RequestPageConfig.PlotMode.DxfExport;

			    this.ExportTypeSelector.Visible = dxfConfig && !User.IsInRole("TEMP");

			    if (RequestPageMode == RequestPageMode.Plot)
				{
					PlotModeMap.GetControl<PlotModeMap>().RotationSliderBehaviourId = ctlPlotLayout.GetControl<PlotLayout>().RotationSliderBehaviourId;
					PlotModeMap.GetControl<PlotModeMap>().hfMapExtentsClientId = hfMapExtents.ClientID;
					ctlPlotLayout.Visible = true;
				}

				if (RequestPageMode == RequestPageMode.Data)
				{
					ctlExportProfileSelector.Visible = DatashopWebConfig.Instance.RequestPageConfig.DataMode.ShowProfileSelection;
					DataModeMap.GetControl<DataModeMap>().hfMapExtentsClientId = hfMapExtents.ClientID;
					ctlRequestDetails.UsePdeToolbar = true;
				}
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Binds the plot sections.
		/// </summary>
		private void BindPlotSections()
		{
			var names = GetPlotSectionsNames();
			if (!string.IsNullOrEmpty(names))
			{
				lblAvailablePlotSection.Text = names;
				placeHolderPlotSection.Visible = true;
			}
			else
				placeHolderPlotSection.Visible = false;
		}

		/// <summary>
		/// Gets the map selections from client.
		/// </summary>
		/// <returns></returns>
		private IList GetMapSelectionsFromClient()
		{
			if (RequestPageMode == RequestPageMode.Plot)
				return GetMapExtentsFromClient();

			if (RequestPageMode == RequestPageMode.Data)
				return GetPerimetersFromClient();

			return null;
		}

		/// <summary>
		/// Gets the process class id.
		/// </summary>
		/// <param name="isDxfExport">if set to <c>true</c> [is DXF export].</param>
		/// <returns></returns>
		private string GetProcessClassId(bool isDxfExport)
		{
			if (RequestPageMode == RequestPageMode.Plot)
			{
				return WorkflowDefinitions.PlotWorkflowClassId;
			}

			if (RequestPageMode == RequestPageMode.Data)
			{
				return isDxfExport
					? WorkflowDefinitions.DxfWorkflowClassId
					: WorkflowDefinitions.PdeWorkflowClassId;
			}

			return null;
		}

		/// <summary>
		/// Gets the model.
		/// </summary>
		/// <param name="mapSelections">The map selections.</param>
		/// <returns></returns>
		private ExportModel GetModel(IList mapSelections)
		{
			if (RequestPageMode == RequestPageMode.Plot)
			{
				var mapExtents = mapSelections as IEnumerable<MapExtent>;
				var model = new PdfExportJobFactory().CreateJob(mapExtents);
				return model;
			}

			if (RequestPageMode == RequestPageMode.Data)
			{
				var perimeters = mapSelections as IEnumerable<ExportPerimeter>;

				var control = ctlExportProfileSelector.GetControl<ExportProfileSelector>();

				return control.OutputFormat == OutputFormat.None
					? new DxfExportJobFactory().CreateJob(control.Profile, perimeters)
					: new PdeExportJobFactory().CreateJob(control.Profile, control.OutputFormat, perimeters);
			}

			return null;
		}

		/// <summary>
		/// Gets the map extents from client for Plot.
		/// </summary>
		private List<MapExtent> GetMapExtentsFromClient()
		{
			var serializer = new JavaScriptSerializer();

			var polygonInfos = serializer.Deserialize<PolygonInfo[]>(ctlRequestDetails.PolygonsInfo);

			if (polygonInfos == null)
				return null;

			var mapExtents = new List<MapExtent>();

			foreach (var polygonInfo in polygonInfos)
			{
				var extent = new MapExtent
				{
					Id = polygonInfo.Id,
					CenterX = polygonInfo.X,
					CenterY = polygonInfo.Y,
					Rotation = polygonInfo.Rotation,
					Scale = polygonInfo.Scale,
					PlotTemplate = polygonInfo.PlotTemplate
				};

				mapExtents.Add(extent);

				LogDebug(string.Format(
								 "A new exportmodel is created (id={0} plotTemplate={1}, x={2}, y={3}, scale={4}, rotation={5}",
								 polygonInfo.Id,
								 polygonInfo.PlotTemplate,
								 polygonInfo.X,
								 polygonInfo.Y,
								 polygonInfo.Scale,
								 polygonInfo.Rotation));
			}

			return mapExtents;
		}


		/// <summary>
		/// Gets the map extents from client for Pde.
		/// </summary>
		private List<ExportPerimeter> GetPerimetersFromClient()
		{
			// surprise : what we get is an array of polygon that is an array of ring that is an array of point that is an array of double.
			// without any attribute name or anything similar. Enjoy.
			var serializer = new JavaScriptSerializer();
			var pdePolygonsJson = ctlRequestDetails.PolygonsInfo;
			var pdePolygons = serializer.Deserialize<double[][][][]>(pdePolygonsJson);
			if (pdePolygons == null)
			{
				return null;
			}

			var perimeters = new List<ExportPerimeter>();
			foreach (var pdePolygon in pdePolygons)
			{
				var perimeter = new ExportPerimeter();

				// apparently there is always only one single ring
				var pdeRing = pdePolygon[0];

				// we also need to remove the last point that is equal to the first one
				var coordinatePairs = from pdePoint in pdeRing.Take(pdeRing.Count() - 1)
									  select new ExportPerimeter.CoordinatePair(pdePoint[0], pdePoint[1]);
				perimeter.PointCollection = coordinatePairs.ToArray();
				perimeters.Add(perimeter);
			}
			return perimeters;
		}

		/// <summary>
		/// Persists the map extents in the session.
		/// </summary>
		private void PersistExtents()
		{
			try
			{
				var serializer = new JavaScriptSerializer();
				var extentInfo = serializer.Deserialize<ExtentInfo>(hfMapExtents.Value);
				Session["lastMapExtents"] = extentInfo;
			}
			catch (Exception ex)
			{
				LogError(string.Format("Error persisting extents to session"), ex);

				Session["lastMapExtents"] = null;
			}
		}

		/// <summary>
		/// Finds any invalid map selections.
		/// </summary>
		/// <param name="mapSelections">The map selections.</param>
		/// <returns></returns>
		private IEnumerable<int> FindAnyInvalidMapSelections(IEnumerable mapSelections)
		{
			if (mapSelections is IList<MapExtent>)
			{
				var extents = mapSelections as IList<MapExtent>;

				var invalidExtents = extents.Where(extent => double.IsNaN(extent.CenterX) || double.IsNaN(extent.CenterY))
											.ToList();

				return invalidExtents.ConvertAll(m => extents.IndexOf(m) + 1);
			}

			return null;
		}

		#endregion

		#region Event handling

		protected void RequestButtonClicked()
		{
			SubmitJob();
		}

		protected void SubmitJob()
		{
			IList mapSelections = null;
			try
			{
				if (RequestPageMode == RequestPageMode.Plot && !ctlPlotLayout.GetControl<PlotLayout>().IsValid)
					return;

				if (RequestPageMode == RequestPageMode.Data && !ctlExportProfileSelector.GetControl<ExportProfileSelector>().IsValid)
				{
					this.ShowMessage(LoadStr(39195, "Please complete the profile selection"));
					return;
				}

				mapSelections = GetMapSelectionsFromClient();

				if (mapSelections == null || mapSelections.Count < 1)
				{
					this.ShowMessage(LoadStr(3807, "Please draw at least one selection."));
					return;
				}

				var invalidExtentIndices = this.FindAnyInvalidMapSelections(mapSelections);

				if (invalidExtentIndices != null && invalidExtentIndices.Any())
				{
					this.ShowMessage(WebLanguage.LoadStr(392666,
						string.Format("There was an unexpected problem with map selection(s): {0}. Try repositioning or replacing them.",
						   string.Join(", ", invalidExtentIndices))));

					return;
				}

				Page.Validate("request");
				if (!Page.IsValid)
					return;

				if (ctlActAsSurrogate.Checked)
				{
					Page.Validate("surrogate");
					if (!Page.IsValid)
						return;
				}

			}
			catch (Exception ex)
			{
				LogError(string.Format("Missing or invalid input parameters for user {0:d}", _userId), ex);
				HttpContext.Current.Session["lasterror"] = ex;
				Response.RedirectSafe("./error/GeneralErrorPage.aspx", false);
			}


			try
			{
				var model = this.GetModel(mapSelections);
				var isDxfExport = model is DxfExportModel;

				var job = new Job
				{
					Definition = model.ToXml(),
					MapExtentCount = model.Perimeters.Length,
					ProcessorClassId = this.GetProcessClassId(isDxfExport),
					ReasonId = Convert.ToInt64(ctlRequestDetails.Reason),
					GeoAttachmentsEnabled = ctlRequestDetails.GeoAttachmentsEnabled,
					UserId = _userId,
					PeriodBeginDate = ctlRequestDetails.PeriodBeginDate,
					PeriodEndDate = ctlRequestDetails.PeriodEndDate,
					Description = ctlRequestDetails.Description,
					ParcelNumber = ctlRequestDetails.ParcelNumber,
					Custom1 = ctlRequestDetails.Custom1 ,
					Custom2 = ctlRequestDetails.Custom2 ,
					Custom3 = ctlRequestDetails.Custom3 ,
					Custom4 = ctlRequestDetails.Custom4 ,
					Custom5 = ctlRequestDetails.Custom5 ,
					Custom6 = ctlRequestDetails.Custom6 ,
					Custom7 = ctlRequestDetails.Custom7 ,
					Custom8 = ctlRequestDetails.Custom8 ,
					Custom9 = ctlRequestDetails.Custom9,
                    Custom10 = ctlRequestDetails.Custom10,
                    DxfExport = ExportTypeSelector.DxfExport
				};

				if (ctlActAsSurrogate.Checked)
				{
					var surrogateJob = new SurrogateJob
					{
						RequestDate = ctlActAsSurrogate.RequestDate,
						RequestType = ctlActAsSurrogate.RequestWay,
						UserId = ctlActAsSurrogate.SurrogateUserId,
						SurrogateUserId = _userId,
						StopAfterProcess = ctlActAsSurrogate.StopAferProcess
					};

					job.SurrogateJob = surrogateJob;
					job.UserId = ctlActAsSurrogate.SurrogateUserId;
				}

				var jobId = DatashopService.Instance.JobService.CreateJob(job);

				Session["jobid"] = jobId;
				Session["userid"] = _userId;

			}
			catch (Exception ex)
			{
				LogError(string.Format("Creating Job failed for user {0:d}", _userId), ex);

				if (ex is FaultException<TooManyPlotsFault>)
				{
					var fex = (FaultException<TooManyPlotsFault>)ex;
					var newMessage = string.Format(LoadStr(10001, ex.Message), fex.Detail.PlotCountInLastMinute,
						fex.Detail.ConfiguredMaxPlotsRate);
					HttpContext.Current.Session["lasterror"] = new Exception(newMessage, fex);
				}
				else
				{
					HttpContext.Current.Session["lasterror"] = ex;
				}
				Response.RedirectSafe("./error/GeneralErrorPage.aspx", false);
				return;
			}

			PersistExtents();
			Response.RedirectSafe("./ConfirmOrder.aspx", false);

		}


		#endregion
	}
}