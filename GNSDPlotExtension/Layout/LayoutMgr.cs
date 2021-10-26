using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.PlotExtension.Utils;
using Point = ESRI.ArcGIS.Geometry.Point;

namespace GEOCOM.GNSD.PlotExtension.Layout
{
    public class LayoutMgr
    {
        // log4net
        private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Sets the extent of the first MapFrame (TODO search with name) found in pagelayout.
        /// </summary>
        /// <param name="centerX">map coordinate x</param>
        /// <param name="centerY">map coordinate y</param>
        /// <param name="scale">desired mapscale</param>
        /// <param name="rotation">rotation in degree 0 = north, clockwise</param>
        internal void SetPlotExtent(double centerX, double centerY, double scale, double rotation)
        {
            IActiveView activeView = PageLayoutManager.Instance.CurrentPageLayout as IActiveView;

            // get the displaytransformation from pagelayout
            IDisplayTransformation displayTransformation = activeView.ScreenDisplay.DisplayTransformation;

            // when used without opening the mxt/mxd with ArcMap, the displaytransformatino is not initialized yet
            // and has the deviceCoordiates set to zero
            displayTransformation.ScaleRatio = 1;
            displayTransformation.ZoomResolution = true;
            displayTransformation.Resolution = 300; // just a value not zero
            displayTransformation.Units = esriUnits.esriCentimeters;
            tagRECT deviceRect = new tagRECT();
            deviceRect.bottom = 678;
            deviceRect.top = 0;
            deviceRect.left = 0;
            deviceRect.right = 1127;
            displayTransformation.set_DeviceFrame(ref deviceRect); // this will automatically calculate the resolution
            displayTransformation.ReferenceScale = 0;
            IEnvelope envelope = new EnvelopeClass();
            envelope.XMin = 5;
            envelope.XMax = 10;
            envelope.YMin = 5;
            envelope.YMax = 10;
            displayTransformation.VisibleBounds = envelope;

            IMapFrame mapFrame = LayerHelper.GetFirstMapFrameFound(PageLayoutManager.Instance.CurrentPageLayout);

            // store old value 
            esriExtentTypeEnum oldExtType = mapFrame.ExtentType;

            IMap map = mapFrame.Map;

            mapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds; // nötig !

            // now we also have to initalize the DisplayTransformatin for the map
            IDisplayTransformation dt = (map as IActiveView).ScreenDisplay.DisplayTransformation;

            // here we can calculate the deviceframe out of the PageLayout
            IEnvelope frameEnv = (mapFrame as IElement).Geometry.Envelope;
            displayTransformation.TransformRect(frameEnv, ref deviceRect, (int)esriDisplayTransformationEnum.esriTransformToDevice);

            // setup transformation 
            dt.set_DeviceFrame(ref deviceRect);

            // here we can set the resolution because the map has set the ZoomResolution Property to false;
            dt.Resolution = displayTransformation.Resolution;

            // set scale and rotation
            // negative sign because the rotation angle comes from extent but we rotate the background and thus we rotate in an opposite direction
            // the angle needs to be normalized otherwise there might be rendering mistakes - more in DATASHOP-615
            dt.Rotation = NormalizeRotation(-rotation); 
            dt.ScaleRatio = scale;

            // Check if successfully set scale and rotation
            _log.DebugFormat("Try setting scale to {0} and rotation to {1}", scale, rotation);
            _log.DebugFormat("Resulting scale is {0} and rotation is {1}", dt.ScaleRatio, dt.Rotation);
            if (Math.Abs(scale - dt.ScaleRatio) > 0.002)
            {
                string msg = "Failed to set mapscale";
                _log.Warn(msg);

                // wif 2010_10: ArcObjects hat teilweise Probleme beim Setzen des Massstabes über die DisplayTransformation.
                // Dieses Problem tritt bei bestimmten Jobs (ca jeder 7.) auf wenn sie unmittelbar nach einem anderen Job ausgeführt werden.
                // Wenn sie anschliessend mit Restart alleine neu ausgeführt werden, ist das Resultat wieder korrekt.
                // Nach dem Setzen des Wertes mit dt.ScaleRatio = scale hat der wert ScaleRation trotzdem einen anderen Wert 
                // als der gesetzte.
                // Aus diesem Grund wird der Massstab bei diesen Fällen über die Bounds gesetzt statt direkt über die ScaleRatio-Eigenschaft.
                // Dies gibt zwar kleine Rundungsfehler, diese liegen aber weit unter dem Toleranzbereich und werden beim Anzeigen des Massstabtextes 
                // auf dem Plot gerundet.
                SetMapScaleWithBounds(dt, scale, rotation);
            }

            // set position
            IEnvelope env = dt.VisibleBounds;

            IPoint centerPoint = new PointClass();
            centerPoint.PutCoords(centerX, centerY);
            env.CenterAt(centerPoint);
            dt.VisibleBounds = env;

            // reset extenttype
            // nötig ?
            mapFrame.ExtentType = oldExtType;

            // Refresh the view (nötig?)
            (map as IActiveView).Refresh();
            (PageLayoutManager.Instance.CurrentPageLayout as IActiveView).Refresh();

            _log.DebugFormat(
                             "Successfully set mapextent to {0} / {1}  width={2} and height={3}.",
                             mapFrame.MapBounds.XMin,
                             mapFrame.MapBounds.XMin,
                             mapFrame.MapBounds.Width,
                             mapFrame.MapBounds.Height);
        }

        /// <summary>
        /// Calculates the mapframe in map coordinates
        /// </summary>
        /// <param name="centerX">The x center coordinates</param>
        /// <param name="centerY">The y center coordinates</param>
        /// <param name="scale">The scale of the plot extent</param>
        /// <param name="rotation">The rotation</param>
        /// <returns>The plotextent hopefully</returns>
        internal IPolygon GetPlotExtent(double centerX, double centerY, double scale, double rotation)
        {
            IMapFrame mapFrame = LayerHelper.GetFirstMapFrameFound(PageLayoutManager.Instance.CurrentPageLayout);
            double width = (mapFrame as IElement).Geometry.Envelope.Width;
            double height = (mapFrame as IElement).Geometry.Envelope.Height;

            return CalcPlotExtend(centerX, centerY, scale, rotation, width, height);
        }

        /// <summary>
        /// Calcs the plot extend.
        /// </summary>
        /// <param name="centerX">The center X.</param>
        /// <param name="centerY">The center Y.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="frameWidth">Width of the frame.</param>
        /// <param name="frameHeight">Height of the frame.</param>
        /// <returns></returns>
        private static IPolygon CalcPlotExtend(double centerX, double centerY, double scale, double rotation, double frameWidth, double frameHeight)
        {
            IPolygon result = new PolygonClass();

            PointF upperLeft = new PointF();
            PointF upperRigth = new PointF();
            PointF lowerLeft = new PointF();
            PointF lowerRigth = new PointF();

            // Calculate extend with center in 0/0
            float width = (float)((frameWidth / 100) * scale);
            float heigth = (float)((frameHeight / 100) * scale);
            upperLeft.X = -width / 2;
            upperLeft.Y = heigth / 2;
            upperRigth.X = width / 2;
            upperRigth.Y = heigth / 2;
            lowerLeft.X = -width / 2;
            lowerLeft.Y = -heigth / 2;
            lowerRigth.X = width / 2;
            lowerRigth.Y = -heigth / 2;

            PointF[] points = { upperLeft, upperRigth, lowerRigth, lowerLeft };

            // Rotate Points
            var matrix = new System.Drawing.Drawing2D.Matrix();

            matrix.Rotate((float)rotation);

            // Rotating with this matrix has the other orientation than the ExtendRotation
            matrix.TransformPoints(points);

            // Add center
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X += (float)centerX;
                points[i].Y += (float)centerY;
            }

            // Create polygon
            object missing = Type.Missing;

            foreach (PointF point in points)
            {
                IPoint pt = new Point();
                pt.PutCoords(point.X, point.Y);
                (result as IPointCollection).AddPoint(pt, ref missing, ref missing);
            }
            result.Close();
            return result;
        }

        /// <summary>
        /// Alternative methode to set the scale ratio if the direkt method with setMapScale failes.
        /// Please remove this workarround if another solution is found.
        /// </summary>
        /// <param name="dt">The display transformation</param>
        /// <param name="scale">The scale to be used</param>
        /// <param name="rotation">The rotation</param>
        private void SetMapScaleWithBounds(IDisplayTransformation dt, double scale, double rotation)
        {
            double factor = scale / dt.ScaleRatio;
            _log.DebugFormat("Scale error factor = {0}", factor);

            // resize the visible bounds to adjust scaleratio
            IEnvelope visibleBounds = dt.VisibleBounds;
            visibleBounds.Expand(factor, factor, true);
            dt.VisibleBounds = visibleBounds;

            // debug output only
            _log.DebugFormat("Resolution = {0}", dt.Resolution);
            var rect = dt.get_DeviceFrame();
            _log.DebugFormat("DeviceFrame = {0} - {1} / {2} - {3}", rect.left, rect.top, rect.right, rect.bottom);
            IEnvelope fb = dt.FittedBounds;
            _log.DebugFormat("FittedBounds = {0} - {1} / {2} - {3} Width={4}", fb.XMin, fb.YMin, fb.XMax, fb.YMax, fb.Width);

            _log.DebugFormat("Rotation = {0}", dt.Rotation);
            _log.DebugFormat("ScaleRatio = {0}", dt.ScaleRatio);
            _log.DebugFormat("ZoomResolution = {0}", dt.ZoomResolution);

            // Check if successfully set scale and rotation
            Assert.True(Math.Abs(scale - dt.ScaleRatio) < 0.002, "Failed to set Mapscale with visible bounds (resulting Scale={0}", dt.ScaleRatio);
        }

        /// <summary>
        /// Converts the rotation to an angle from interval &lt;0, 360)
        /// </summary>
        /// <param name="rotation">Any rotation angle</param>
        /// <returns>Normalized angle from interval &lt;0, 360)</returns>
        private static double NormalizeRotation(double rotation)
        {
            double normalizedRotation = rotation % 360;
            return 0 <= normalizedRotation ? normalizedRotation : 360 + normalizedRotation;
        }
    }
}
