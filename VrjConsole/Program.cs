using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.PlotExtension.Utils;


namespace VrjConsole
{
    public class Program
	{
	    private static void Main(string[] args)
        {
            var black = Color.Black;
            //black.R = 0;
            return;
	        var userStore = new UserStore();

	  
	        var jobDetailsStore = new JobDetailsStore();
	        var startNew = Stopwatch.StartNew();

	        string[] templates =
	        {
	            "A3_hoch",
	            "A3_quer",
	            "A4_hoch",
	            "A4_quer"
	        };

	        for (int i = 0; i < 30; i++)
	        {

                var byId = userStore.GetById(i);
                if(byId == null)
                    continue;
	        foreach (var template in templates)
	        {
              
                var plotCountInTimePeriodForUserAndTemplate = jobDetailsStore.GetPlotCountInTimePeriodForUserAndTemplate(byId, template, new DateTime(2014, 10, 07));
                var plotCountInTimePeriodForUserAndTemplate_old = jobDetailsStore.GetPlotCountInTimePeriodForUserAndTemplate_old(byId, template, new DateTime(2014, 10, 07));
	            if (plotCountInTimePeriodForUserAndTemplate_old != plotCountInTimePeriodForUserAndTemplate)
	            {
	                Console.WriteLine();
	            }
	            else
	            {
	                Console.WriteLine("match");
	            }
            }
            }

           
            for (int i = 0; i <1; i++)
            {
                var byId = userStore.GetById(1);
                var plotCountInTimePeriodForUserAndTemplate = jobDetailsStore.GetPlotCountInTimePeriodForUserAndTemplate(byId, "A3_hoch", new DateTime(2014, 10, 07));
            }
            Console.WriteLine(startNew.Elapsed);
            startNew = Stopwatch.StartNew();
            for (int i = 0; i < 1; i++)
            {
                var byId = userStore.GetById(1);
                var plotCountInTimePeriodForUserAndTemplate = jobDetailsStore.GetPlotCountInTimePeriodForUserAndTemplate_old(byId, "A3_hoch", new DateTime(2014, 10, 07));
            }
            Console.WriteLine(startNew.Elapsed);

	    }

	    [STAThread]
		private static void Mai2n(string[] args)
		{
			var format = string.Format(CultureInfo.InvariantCulture, "{0}", 1.01);
			return;
			RuntimeManager.BindLicense(ProductCode.EngineOrDesktop);

			var mapDocumentClass = new MapDocumentClass();
			mapDocumentClass.Open(@"d:\Development\Datashop\Maps\gnsd_plot_wms.mxd", null);
			var invalidLayers = LayerHelper.GetInvalidLayers(mapDocumentClass.Map[0]);
			return;
			//IsWMSLayersValid
			var program = new Program();
			var pageLayout = program.InsertMapToPlotTemplate(
				@"d:\Development\Datashop\Installation\Server\plottemplates\A4_hoch.mxt",
				@"D:\Development\Datashop\Maps\GNSD_ALL.mxd");
			program.SetExtent(pageLayout as IActiveView, 708766.9291338583, 233401.57480314959, 25000, 40);
			program.ExportMapToPdf(pageLayout, @"d:\export2.pdf");
		}

		private void SetExtent(IActiveView activeView, double centerX, double centerY, double scale, double rotation)
		{
			// get the displaytransformation from pagelayout
			IDisplayTransformation displayTransformation = activeView.ScreenDisplay.DisplayTransformation;

			// when used without opening the mxt/mxd with ArcMap, the display transformation is not initialized yet
			// and has the device coordinates set to zero
			displayTransformation.ScaleRatio = 1;
			displayTransformation.ZoomResolution = true;
			displayTransformation.Resolution = 1; // just a value not zero
			displayTransformation.Units = esriUnits.esriCentimeters;
			tagRECT deviceRect = new tagRECT
			{
				top = 0, 
				left = 0, 
				bottom = 1, 
				right = 1
			};
			displayTransformation.set_DeviceFrame(ref deviceRect); // this will automatically calculate the resolution
			displayTransformation.ReferenceScale = 0;
			EnvelopeClass envelope = new EnvelopeClass
			{
				XMin = 0,
				YMin = 0, 
				XMax = 1, 
				YMax = 1
			};
			displayTransformation.VisibleBounds = envelope;

			IMapFrame mapFrame = GetFirstMapFrameFound((IPageLayout)activeView);
			mapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;

			// now we also have to initialize the Display Transformation for the map
			IDisplayTransformation dt = ((IActiveView)mapFrame.Map).ScreenDisplay.DisplayTransformation;

			// here we can calculate the deviceframe out of the PageLayout
			IEnvelope frameEnv = (mapFrame as IElement).Geometry.Envelope;
			displayTransformation.TransformRect(frameEnv, ref deviceRect, (int)esriDisplayTransformationEnum.esriTransformToDevice);

			// setup transformation 
			dt.set_DeviceFrame(ref deviceRect);

			// here we can set the resolution because the map has set the ZoomResolution Property to false;
			dt.Resolution = displayTransformation.Resolution;

			// set scale and rotation
			dt.Rotation = -rotation;
			dt.ScaleRatio = scale;

			// set position
			IEnvelope env = dt.VisibleBounds;
			IPoint centerPoint = new PointClass();
			centerPoint.PutCoords(centerX, centerY);
			env.CenterAt(centerPoint);
			dt.VisibleBounds = env;
			activeView.Refresh();
		}

		private void ExportMapToPdf(IPageLayout pageLayout, string filename, int resolution = 600)
		{
			pageLayout.Page.Units = esriUnits.esriCentimeters;
			double width, height;
			pageLayout.Page.QuerySize(out width, out height);
			var pixelExtent = new tagRECT
			{
				right = (int)(width * resolution / 2.54),
				bottom = (int)(height * resolution / 2.54)
			};

			IEnvelope env = new EnvelopeClass();
			env.PutCoords(0, 0, pixelExtent.right, pixelExtent.bottom);

			if (File.Exists(filename))
				File.Delete(filename);

			ExportPDFClass pdfExporter = new ExportPDFClass
			{
				Compressed = true,
				ImageCompression = esriExportImageCompression.esriExportImageCompressionLZW,
				EmbedFonts = true,
				ExportPDFLayersAndFeatureAttributes = esriExportPDFLayerOptions.esriExportPDFLayerOptionsNone,
				ExportMeasureInfo = true,
				PolygonizeMarkers = false,
				ExportPictureSymbolOptions = esriPictureSymbolOptions.esriPSORasterizeIfRasterData,
				Colorspace = esriExportColorspace.esriExportColorspaceRGB,
				Resolution = resolution,
				ExportFileName = filename,
				PixelBounds = env
			};


			var activeView = (IActiveView)pageLayout;
			activeView.Refresh();

			int hDC = pdfExporter.StartExporting();

			activeView.Output(hDC, resolution, ref pixelExtent, null, null);

			pdfExporter.FinishExporting();
			pdfExporter.Cleanup();

		}

		private IPageLayout InsertMapToPlotTemplate(string plotTemplatePath, string mapPath)
		{
			var serverMapDocument = new MapDocumentClass();
			var plotTemplateDocument = new MapDocumentClass();
			try
			{
				plotTemplateDocument.Open(plotTemplatePath);
				serverMapDocument.Open(mapPath);
				var currentPageLayout = plotTemplateDocument.PageLayout;

				IMapFrame mapFrame = GetFirstMapFrameFound(currentPageLayout);
				IMap serverMap = serverMapDocument.Map[0];
				plotTemplateDocument.ReplaceContents(serverMap as IMxdContents);
				mapFrame.Map = serverMap;
				ReplaceMapForMapSurrounds(currentPageLayout, mapFrame);
				mapFrame.Map.SpatialReference = serverMap.SpatialReference;
				return currentPageLayout;
			}
			finally
			{
				serverMapDocument.Close();
				plotTemplateDocument.Close();
			}
		}

		private IMapFrame GetFirstMapFrameFound(IPageLayout pageLayout)
		{
			IGraphicsContainer gc = pageLayout as IGraphicsContainer;
			gc.Reset();
			IElement element = gc.Next();
			while (element != null)
			{
				var frame = element as IMapFrame;
				if (frame != null)
					return frame;
				element = gc.Next();
			}
			throw new Exception("No mapframe found in pagelayout");
		}

		private void ReplaceMapForMapSurrounds(IPageLayout plotTemplatePageLayout, IMapFrame mapFrame)
		{
			IGraphicsContainer gc = plotTemplatePageLayout as IGraphicsContainer;
			gc.Reset();
			IElement element = null;

			while ((element = gc.Next()) != null)
			{
				if (element is IMapSurroundFrame)
				{
					SetMapSurroundOnElement(element, mapFrame);
				}
				else if (element is IGroupElement)
				{
					IGroupElement group = element as IGroupElement;
					IElement groupItem;
					IEnumElement elements = group.Elements;
					while ((groupItem = elements.Next()) != null)
					{
						if (groupItem is IMapSurroundFrame)
						{
							SetMapSurroundOnElement(groupItem, mapFrame);
						}
					}
				}
			}
		}

		private void SetMapSurroundOnElement(IElement element, IMapFrame mapFrame)
		{
			IMapSurround mapSurround = (element as IMapSurroundFrame).MapSurround;
			if (mapSurround != null)
			{
				mapSurround.Map = mapFrame.Map;
			}

		}


	}
}

