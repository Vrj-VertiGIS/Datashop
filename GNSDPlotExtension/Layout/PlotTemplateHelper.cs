using System;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.PlotExtension.Utils;
using Path = System.IO.Path;

namespace GEOCOM.GNSD.PlotExtension.Layout
{
    public class PlotTemplateHelper
    {
        /// <summary>
        /// Gets the full template name. Checks whether an *.mxt or a *.mxd file exists with the given template name.
        /// </summary>
        /// <param name="templateName">The template name or path</param>
        /// <returns>The full file name of the template (either with extension .mxt or .mxd)</returns>
        public static String GetTemplateFullName(String templateName)
        {
            if (File.Exists(templateName))
                return templateName;

            FileInfo fullFileName;
            String mxtFileName = Path.Combine(PlotExtensionConfig.Instance.Export.PlotTemplate, String.Format("{0}.{1}", templateName, "mxt"));
            String mxdFileName = Path.ChangeExtension(mxtFileName, "mxd");
            if (File.Exists(mxtFileName))
            {
                fullFileName = new FileInfo(mxtFileName);
            }
            else if (File.Exists(mxdFileName))
            {
                fullFileName = new FileInfo(mxdFileName);
            }
            else
            {
                throw new Exception(string.Format("Invalid configuration. PlotTemplate={0} doesn't exist.", templateName));
            }
            return fullFileName.FullName;
        }


        /// <summary>
        /// Used to write the mapframe width and heigth to db.
        /// Opens the mxd and 
        /// </summary>
        /// <returns>The envelope of the first frame found</returns>
        public static IEnvelope GetMapExtentInPageCoords(string templateName)
        {
            IMapDocument plotTemplateDocument = new MapDocumentClass();
            IEnvelope mapExtentEnvelope;
            string templateFullName = GetTemplateFullName(templateName);

            plotTemplateDocument.Open(templateFullName, string.Empty);
            IMapFrame mapFrame = LayerHelper.GetFirstMapFrameFound(plotTemplateDocument.PageLayout);
            mapExtentEnvelope = (mapFrame as IElement).Geometry.Envelope;
            plotTemplateDocument.Close();
            return mapExtentEnvelope;
        }
    }
}
