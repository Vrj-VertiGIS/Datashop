using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web.Script.Serialization;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.ServiceClient;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSD.Web.Core.JSSerializer
{
    /// <summary>
    /// Class that serializes the export models to JSON
    /// </summary>
    public class ExportModelJSSerializer
    {
        /// <summary>
        /// Gets or sets the rings.
        /// </summary>
        /// <value>
        /// The rings.
        /// </value>
        public float[][][] rings { get; set; }

        /// <summary>
        /// Serializes the extend model.
        /// </summary>
        /// <param name="extendModel">The extend model.</param>
        /// <returns></returns>
        public static string SerializeExtendModel(ExportModel extendModel)
        {
            var serializers = new ExportModelJSSerializer[extendModel.Perimeters.Length];
            for (int i = 0; i < extendModel.Perimeters.Length; i++)
            {
                var ser = new ExportModelJSSerializer();
                if (extendModel is TdeExportModel)
                    ser.CalcExportExtend(extendModel.Perimeters[i]);
                else
                    ser.CalcPlotExtend(extendModel.Perimeters[i]);
                serializers[i] = ser;
            }
            var serializer = new JavaScriptSerializer();

            return serializer.Serialize(serializers);
        }

        /// <summary>
        /// Calcs the plot extend.
        /// </summary>
        /// <param name="perimeter">The perimeter.</param>
        public void CalcPlotExtend(ExportPerimeter perimeter)
        {
            rings = new float[1][][];

            var extent = perimeter.MapExtent;

            if (extent != null)
            {
                 IJobManager jobManager = DatashopService.Instance.JobService;
                Plotdefinition[] templates = jobManager.GetAllTemplates();

                Plotdefinition template = templates.Where(t => t.PlotdefinitionKey.Template.Equals(extent.PlotTemplate, StringComparison.InvariantCultureIgnoreCase))
                        .DefaultIfEmpty(new Plotdefinition { PlotHeightCm = 30, PlotWidthCm = 30 })
                        .FirstOrDefault();

                //callculate extend with center in 0/0
                var width = (float)((template.PlotWidthCm / 100) * extent.Scale);
                var height = (float)((template.PlotHeightCm / 100) * extent.Scale);

                var points = new[]{
                                     new PointF { X = -width / 2, Y = height / 2 }, 
                                     new PointF { X = width / 2, Y = height / 2 }, 
                                     new PointF { X = width / 2, Y = -height / 2 }, 
                                     new PointF { X = -width / 2, Y = -height / 2 }
                                 };
                //rotate Points
                var matrix = new Matrix();
                matrix.Rotate((float)extent.Rotation);
                //rotating with this matrix has the other orientation than the ExtendRotation
                matrix.TransformPoints(points);

                //add Centrum
                for (var j = 0; j < points.Length; j++)
                {
                    points[j].X += (float)extent.CenterX;
                    points[j].Y += (float)extent.CenterY;
                }

                rings[0] = new float[5][];

                rings[0][0] = new[] { points[0].X, points[0].Y };
                rings[0][1] = new[] { points[1].X, points[1].Y };
                rings[0][2] = new[] { points[2].X, points[2].Y };
                rings[0][3] = new[] { points[3].X, points[3].Y };
                rings[0][4] = new[] { points[0].X, points[0].Y };
            }
        }

        /// <summary>
        /// Calcs the export extend.
        /// </summary>
        /// <param name="perimeter">The perimeter.</param>
        public void CalcExportExtend(ExportPerimeter perimeter)
        {
            rings = new float[1][][];
            if (perimeter.PointCollection != null)
            {
                var maxPairs = perimeter.PointCollection.Count();
                rings[0] = new float[maxPairs + 1][];
                ExportPerimeter.CoordinatePair pair;
                for (var i = 0; i < maxPairs; i++)
                {
                    pair = perimeter.PointCollection[i];
                    rings[0][i] = new[] { (float)pair.X, (float)pair.Y };
                }
                // close the ring
                pair = perimeter.PointCollection[0];
                rings[0][maxPairs] = new[] { (float)pair.X, (float)pair.Y };
            }
        }
    }
}