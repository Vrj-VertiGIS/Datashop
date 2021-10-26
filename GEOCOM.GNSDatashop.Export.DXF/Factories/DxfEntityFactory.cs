using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.TTF;
using netDxf;
using netDxf.Blocks;
using netDxf.Tables;
using System.Collections.Generic;
using System.Linq;
using Entities = netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.Factories
{
    public abstract class DxfEntityFactory 
    {
        private Layer _layer = null;

        protected double _dotsToMeter; // scale to convert from dots (points - 1/72 of an inch) - to meter

        #region construction/dstruction

        public static DxfEntityFactory CreateFactory(Layer layer, bool is3Ddata, double dotsToMeter)
        {
            return (is3Ddata)
                ? new Dxf3DEntityFactory(layer, dotsToMeter) as DxfEntityFactory
                : new Dxf2DEntityFaxtory(layer, dotsToMeter) as DxfEntityFactory;
        }

        protected DxfEntityFactory(Layer layer, double dotsToMeter)
        {
            _layer = layer;
            _dotsToMeter = dotsToMeter;
        }

        #endregion

        #region Insert (Block insert)

        /// <summary>
        /// Create dxf block insert - insert (font) symbol (reference) into dxf
        /// </summary>
        /// <param name="block"></param>
        /// <param name="rotation"></param>
        /// <param name="point"></param>
        /// <param name="symbolInfo"></param>
        /// <returns></returns>
        public Entities.EntityObject CreateBlockInsert(Block block, double rotation, IPoint point, SymbolInfo symbolInfo)
            => CreateBlockInsert(block, rotation, point, symbolInfo.DxfTransparency);

        /// <summary>
        /// Create dxf block insert - insert (font) symbol (reference) into dxf
        /// </summary>
        /// <param name="block"></param>
        /// <param name="rotation"></param>
        /// <param name="point"></param>
        /// <param name="dxfTransparency">In a layered symbol, transparency is the sum of all transparencies.</param>
        /// <returns></returns>
        public Entities.EntityObject CreateBlockInsert(Block block, double rotation, IPoint point, short dxfTransparency)
        {
            var insert = CreateBlockInsertCore(block, point);

            insert.Color = AciColor.ByBlock;
            insert.Transparency = new Transparency(dxfTransparency);
            insert.Scale = new Vector3(_dotsToMeter, _dotsToMeter, _dotsToMeter);
            insert.Rotation = rotation;

            PopulateCommonFields(insert);

            return insert;
        }

        protected abstract Entities.Insert CreateBlockInsertCore(Block block, IPoint point);
        #endregion

        #region Hatch

        /// <summary>
        /// Create a dxf hatch (polygon fill)
        /// </summary>
        /// <param name="rings"></param>
        /// <param name="symbolInfo"></param>
        /// <returns></returns>
        public Entities.Hatch CreateHatch(IEnumerable<Entities.EntityObject> rings, FillSymbolInfo symbolInfo)
        {
            if (symbolInfo.IsVisible)
            {
                var hb  = CreateHatchCore(rings);
                var hatchBoundaries = hb.ToList();

                var hatch = new Entities.Hatch(symbolInfo.Pattern, hatchBoundaries, false);
                hatch.Color = symbolInfo.DXFColor;
                hatch.Transparency = new Transparency(symbolInfo.DxfTransparency);

                hatch.Lineweight = DxfLineweights.FromMeasure(symbolInfo.PatternlineWidth);

                PopulateCommonFields(hatch);

                return hatch;
            }
            return null;
        }

        protected abstract IEnumerable<Entities.HatchBoundaryPath> CreateHatchCore(IEnumerable<Entities.EntityObject> rings);

        #endregion

        #region Polyline/LWPolyline

        /// <summary>
        /// Create polyline(s) representing one esri curve and the related symbol layers. In Dxf, there are no
        /// symbol layers so we need to create a dxf (lw)polyline for each esri/arcObjects symbol layer.
        /// </summary>
        /// <param name="symbolLayers"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<Entities.EntityObject>> CreatePolylineGeometries(LayeredLineSymbolInfo symbolLayers, ICurve curve)
        {
            foreach (var symbolInfoLayer in symbolLayers.LayersByVisibility.Reverse())
                yield return CreatePolyline(curve, symbolInfoLayer);
        }

        /// <summary>
        /// Create a polyline with entire symbology (line decoration, line markers etc.)
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="symbolInfo"></param>
        /// <returns></returns>
        //
        public IEnumerable<Entities.EntityObject> CreatePolyline(ICurve curve, ILineSymbolInfo symbolInfo)
        {
            return (symbolInfo is LayeredLineSymbolInfo llsi)
                ? CreatePolyline(curve, llsi)
                : CreatePolyline(curve, symbolInfo as LineSymbolInfo);
        }

        private IEnumerable<Entities.EntityObject> CreatePolyline(ICurve curve, LayeredLineSymbolInfo symbolInfo)
        {
            var entities = new List<Entities.EntityObject>();
            foreach (var symbolInfoLyer in symbolInfo)
                entities.AddRange(CreatePolyline(curve, symbolInfoLyer));
            return entities;
        }

        private IEnumerable<Entities.EntityObject> CreatePolyline(ICurve curve, LineSymbolInfo symbolInfo)
        {
            if (curve is ISegment segment)
                return CreatePolyline(segment, symbolInfo);
            if (curve is IPolycurve polyCurve)
                return CreatePolyline(polyCurve, symbolInfo);
            else
                return new List<Entities.EntityObject>();
        }

        private IEnumerable<Entities.EntityObject> CreatePolyline(IPolycurve polyCurve, LineSymbolInfo symbolInfo)
        {
            var entityObjects = new List<Entities.EntityObject>();

            if (!polyCurve.IsEmpty)
            {
                if (this is Dxf3DEntityFactory)
                    DensifyGeometry(polyCurve as IGeometry);

                var polyGeometryInfo = new PolyCurveInfo(polyCurve, symbolInfo.Offset * _dotsToMeter);

                entityObjects.AddRange(CreateLineDecoration(polyGeometryInfo.Curve as ICurve, symbolInfo, true));
                entityObjects.AddRange(CreatePolyline(polyGeometryInfo, symbolInfo));
                entityObjects.AddRange(CreateLineDecoration(polyGeometryInfo.Curve as ICurve, symbolInfo, false));
            }

            return entityObjects;
        }

        private IEnumerable<Entities.EntityObject> CreatePolyline(PolyCurveInfo polyGeometryInfo, LineSymbolInfo symbolInfo)
        {
            if (symbolInfo is LinetypeLineSymbolInfo ltsi)
            {
                foreach (var path in polyGeometryInfo.Paths)
                    yield return CreatePolylinePath(_layer, path.Points, ltsi, polyGeometryInfo.ClosedRings);
            }
            else if (symbolInfo is MarkerLineSymbolInfo mls)
            {
                foreach (ICurve curve in polyGeometryInfo.Geometries)
                    foreach (var marker in mls.Template.GetMarks(curve, _dotsToMeter))
                        yield return CreateLineMarker(mls.SymbolInfo, marker, true, false);
            }
        }

        private IEnumerable<Entities.EntityObject> CreatePolyline(ISegment segment, LineSymbolInfo symbolInfo)
        {
            var pl = new PolylineClass() as ISegmentCollection;
            pl.AddSegment(segment);
            return CreatePolyline(pl as IPolycurve, symbolInfo);
        }

        private Entities.EntityObject CreatePolylinePath(Layer dxfLyr, IEnumerable<IPointInfo> pathPoints, LinetypeLineSymbolInfo symbolInfo, bool closedRings)
        {
            var entity = CreatePolylineCore(pathPoints, symbolInfo, closedRings);
            entity.Color = symbolInfo.DXFColor;
            entity.Transparency = new Transparency(symbolInfo.DxfTransparency);
            entity.Linetype = symbolInfo.LineType;
            PopulateCommonFields(entity);
            return entity;
        }

        private IEnumerable<Entities.EntityObject> CreateLineDecoration(ICurve curve, LineSymbolInfo symbolLayer, bool ontopDecoration)
        {
            var decoration = symbolLayer.LineDecoration;
            var layers = decoration.DecorationLayers;

            return (decoration.Ontop == ontopDecoration) && layers.Any()
                ? CreateLineDecoration(layers, curve)
                : new List<Entities.EntityObject>();
        }

        private IEnumerable<Entities.EntityObject> CreateLineDecoration(IEnumerable<LineDecorationLayer> layers, ICurve curve)
        {
            foreach (var layer in layers)
            {
                var allPositions = layer.GetPositions(curve);
                var thisPosition = allPositions.First();
                var flipThis = (thisPosition.IsFirst && (layer.FlipFirst ^ layer.FlipAll)) || (!thisPosition.IsFirst && layer.FlipAll);
                yield return CreateLineMarker(layer.MarkerSymbolInfo, thisPosition, layer.Rotate, flipThis);
                foreach (var position in allPositions.Skip(1))
                    yield return CreateLineMarker(layer.MarkerSymbolInfo, position, layer.Rotate, layer.FlipAll);
            }
        }

        protected abstract Entities.EntityObject CreatePolylineCore(IEnumerable<IPointInfo> points, LinetypeLineSymbolInfo symbology, bool closedRings);

        /// <summary>
        /// The whole job is to densify the curved segments only - leaving straight line segments untouched
        /// </summary>
        /// <param name="geometry">geometry to densify</param>
        private void DensifyGeometry(IGeometry geometry)
        {
            var geometryCollection = geometry as IGeometryCollection;
            if (null != geometryCollection)
            {
                for (int i = 0; (i < geometryCollection.GeometryCount - 1); i++)
                    DensifyGeometry(geometryCollection.Geometry[i]);
            }
            else
            {
                var segmentCollection = geometry as ISegmentCollection;
                if (null != segmentCollection)
                {
                    int tail = segmentCollection.SegmentCount - 1;
                    while (tail >= 0)
                    {
                        if (!(segmentCollection.Segment[tail] is ILine))
                            tail--;
                        else
                        {
                            var curvedSegments = new Polyline() as ISegmentCollection;
                            curvedSegments.AddSegment(segmentCollection.Segment[tail]);
                            int head = tail - 1;
                            for (; ((head >= 0) && (!(segmentCollection.Segment[head] is ILine))); head--)
                                curvedSegments.AddSegment(segmentCollection.Segment[head], segmentCollection.Segment[0]);
                            (curvedSegments as IPolycurve).Densify(0, 0);   // Use the defaults heere
                            segmentCollection.ReplaceSegmentCollection((head >= 0) ? head : 0, tail - head, curvedSegments);
                            tail = head;
                        }
                    }
                }
            }
        }

        private Entities.EntityObject CreateLineMarker(MarkerSymbolInfo markerSymbol, LineMarkerPosition position, bool rotate, bool flip)
        {
            var rotation = GetLineMarkerRotation(markerSymbol, position, rotate, flip);

            return CreateBlockInsert(markerSymbol.Block.Block, rotation, position.ESRIPosition, markerSymbol);
        }

        private static double GetLineMarkerRotation(MarkerSymbolInfo markerSymbol, LineMarkerPosition position, bool rotate, bool flip)
        {
            var rotation = markerSymbol.Rotation
                + ((rotate) ? position.Tangent : 0.0)
                + ((flip) ? 180 : 0);
            while (rotation > 360)
                rotation -= 360;
            while (rotation < 0)
                rotation += 360;
            return rotation;
        }

        #endregion

        #region MText

        /// <summary>
        /// Create dxf text
        /// </summary>
        /// <param name="symbolInfo"></param>
        /// <returns></returns>
        public Entities.MText CreateMText(TextSymbolInfo symbolInfo)
        {
            var mText = new MTextLabelTokenWriter(_layer, symbolInfo, _dotsToMeter);
            mText.Height = symbolInfo.TextStyle.Height;
            mText.Position = new Vector3(symbolInfo.ReferencePoint.X + _dotsToMeter * symbolInfo.XOffset,
                                         symbolInfo.ReferencePoint.Y + _dotsToMeter * symbolInfo.YOffset, 0.0);

            mText.Color = symbolInfo.DXFColor;
            mText.Style.WidthFactor = symbolInfo.CharacterWidth / 100.0;    // CharacterWidth is given in %
            mText.Rotation = symbolInfo.Angle;

            mText.AttachmentPoint = symbolInfo.Alignment.DxfAttachementPoint;
            mText.RectangleWidth = 0.0; // "Manual" (explicit) line wrap by text markup (i.e. \n, \P)

            mText.Transparency = new Transparency(symbolInfo.DxfTransparency);

            mText.Write(symbolInfo.TextTokens);

            return mText;
        }

        /// <summary>
        /// Create dxf text
        /// </summary>
        /// <param name="symbolInfo"></param>
        /// <param name="textToWrite">The text string to be written</param>
        /// <returns></returns>
        public Entities.MText CreateMText(TextSymbolInfo symbolInfo, string textToWrite)
        {
            symbolInfo.Set_Text(textToWrite);
            return CreateMText(symbolInfo);
        }

        #endregion

        #region private helpers

        private void PopulateCommonFields(Entities.EntityObject entity)
        {
            entity.Layer = _layer;
        }

        #endregion

    }
}