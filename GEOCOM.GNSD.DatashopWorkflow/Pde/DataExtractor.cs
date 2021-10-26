using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.Model;

using Path = System.IO.Path;

namespace GEOCOM.GNSD.DatashopWorkflow.Pde
{
    public class DataExtractor
    {
        private readonly IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly TdeExportModel _tdeExportModel;

        #region Constructor

        internal DataExtractor(TdeExportModel tdeExportModel)
        {
            _tdeExportModel = tdeExportModel;
        }

        #endregion

        public List<IGeometry> GetJobExtentList()
        {
            return PerimetersToGeometries();
        }

        private List<IGeometry> PerimetersToGeometries()
        {
            //// var perimeter = model.Perimeter;
            List<IGeometry> geometries = new List<IGeometry>();
            foreach (var perimeter in _tdeExportModel.Perimeters)
            {
                if (perimeter == null)
                    return null;

                var extent = perimeter.MapExtent;

                if (extent != null)
                {
                    PointF upperLeft = new PointF();
                    PointF upperRigth = new PointF();
                    PointF lowerLeft = new PointF();
                    PointF lowerRigth = new PointF();

                    // get correct Plottemplate
                    // TODO <2010_02_26/ wif / prio3> Get Plottemplate
                    PlotTemplate template = new PlotTemplate() { PlotHeightCm = 25, PlotWidthCm = 20 };

                    // callculate extend with center in 0/0
                    float width = (float)((template.PlotWidthCm / 100) * extent.Scale);
                    float heigth = (float)((template.PlotHeightCm / 100) * extent.Scale);
                    upperLeft.X = -width / 2;
                    upperLeft.Y = heigth / 2;
                    upperRigth.X = width / 2;
                    upperRigth.Y = heigth / 2;
                    lowerLeft.X = -width / 2;
                    lowerLeft.Y = -heigth / 2;
                    lowerRigth.X = width / 2;
                    lowerRigth.Y = -heigth / 2;

                    PointF[] points = { upperLeft, upperRigth, lowerRigth, lowerLeft };

                    // rotate Points
                    var matrix = new System.Drawing.Drawing2D.Matrix();

                    matrix.Rotate((float)-extent.Rotation);

                    // rotating with this matrix has the other orientation than the ExtendRotation
                    matrix.TransformPoints(points);

                    List<WKSPoint> wksPoints = new List<WKSPoint>();

                    // add Centrum
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i].X += (float)extent.CenterX;
                        points[i].Y += (float)extent.CenterY;
                        wksPoints.Add(new WKSPoint() { X = points[i].X, Y = points[i].Y });
                    }

                    var polygon = Polygon(wksPoints, null);
                    geometries.Add(polygon);

                    // TODO <2010_02_26/ wif / prio3> Spatialref irgendwo herholen
                }
                else
                {
                    var pointCollection = perimeter.PointCollection;
                    if (pointCollection != null)
                    {
                        List<WKSPoint> wksPoints = new List<WKSPoint>();
                        foreach (ExportPerimeter.CoordinatePair coordinatePair in pointCollection)
                        {
                            wksPoints.Add(new WKSPoint() { X = coordinatePair.X, Y = coordinatePair.Y });
                        }
                        var polygon = Polygon(wksPoints, null);
                        geometries.Add(polygon);
                    }
                }
            }

            return geometries;
        }

        private IPolygon Polygon(IEnumerable<WKSPoint> wksPoints, ISpatialReference spatialReference)
        {
            Assert.NotNull(wksPoints, "wksPoints");
            IPolygon polygon =  new PolygonClass();
            
            WKSPoint[] wksPointsArray = wksPoints as WKSPoint[] ?? wksPoints.ToArray();
            var geometryEnvironmentClass = new GeometryEnvironmentClass();
            geometryEnvironmentClass.AddWKSPoints((IPointCollection4)polygon, ref wksPointsArray);
            if (!polygon.IsClosed)
                polygon.Close();
 
            return polygon;
        }

        private class PlotTemplate
        {
            public double PlotWidthCm { get; set; }

            public double PlotHeightCm { get; set; }
        }
    }
}
