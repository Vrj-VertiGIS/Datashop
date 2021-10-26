using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.TTF;
using log4net;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class DxfBlockFactory 
    {
        private IntPtr _memDC = IntPtr.Zero;
        private Layer _dxfLayer = null;
        private IDisplayTransformation _displayTransformation = null;

        private delegate EntityObject AtomicGeometryCreator(Vector2 position, params double[] sizes);
        private delegate List<EntityObject> CompoundGeometryCreator(Vector2 position, params double[] sizes);

        private static readonly log4net.ILog _log = LogManager.GetLogger("DxfWriter");

        public DxfBlockFactory(IDisplayTransformation displayTransformation, IntPtr hDC, Layer dxfLayer)
        {
            _displayTransformation = displayTransformation;
            _memDC = WinAPI.CreateCompatibleDC(hDC);
            _dxfLayer = dxfLayer;
        }

        ~DxfBlockFactory()
            => DisposeUnmanaged();

        private void DisposeUnmanaged()
        {
            if (_memDC != IntPtr.Zero)
            {
                WinAPI.DeleteDC(_memDC);
                _memDC = IntPtr.Zero;
            }
        }

        public BlockInfo CreateBlock(string name, ISymbol symbol, GlyphAlignment symbolAlignment)
        {
            return CreateBlock(name, CreateEntitiesForSymbol(name, symbol, symbolAlignment));
        }

        private BlockInfo CreateBlock(string name, EntityObjectInfo entities)
        {
            var block = new Block(name, entities.Objects)
            {
                Layer = _dxfLayer
            };

            return new BlockInfo(block, entities.Extent);
        }

        private EntityObjectInfo CreateEntitiesForSymbol(string symbolName, ISymbol symbol, GlyphAlignment symbolAlignment)
        {
            if (symbol == null)
                return new EntityObjectInfo();

            var markerSymbol = (symbol as IClone).Clone() as IMarkerSymbol;

            if (markerSymbol != null)
            {
                markerSymbol.Angle = 0.0;   // We want all symbols unrotated. The symbol rotation will be done upon DXF block INSERT()
                
                var entities = CreateEntitiesForMarkerSymbol(markerSymbol, symbolAlignment);

#if DEBUG
                //_log.Info($"Symbol (geometry) trace: {symbolName}");
                //foreach (var l in entities.ToStrings()) _log.Info(l);
#endif

                return entities;
            }
            else
                throw new NotSupportedException("Can only process MultiLayer-, Character- and Simple- Marker symbols.");
        }

        private EntityObjectInfo CreateEntitiesForMarkerSymbol(IMarkerSymbol symbol, GlyphAlignment symbolAlignment, double xOffset = 0.0, double yOffset = 0.0)
        {
            xOffset += symbol.XOffset;
            yOffset += symbol.YOffset;

            if (symbol is IMultiLayerMarkerSymbol multiLayer)
                return CreateEntitiesForMultiLayeMarker(multiLayer, symbolAlignment, xOffset, yOffset);
            else if (symbol is ISimpleMarkerSymbol simpleMarkerSymbol)
            {
                if (!(simpleMarkerSymbol.Color.IsTransparent() && simpleMarkerSymbol.OutlineColor.IsTransparent()))
                    return CreateEntitiesForSimpleMarker(simpleMarkerSymbol, symbolAlignment);
            }
            else if (symbol is IArrowMarkerSymbol arrowMarkerSymbol)
            {
                if (!arrowMarkerSymbol.Color.IsTransparent())
                    return CreateEntitiesForArrowMarker(arrowMarkerSymbol, symbolAlignment);
            }
            else if (symbol is ICharacterMarkerSymbol characterMarkerSymbol)
            {
                if (characterMarkerSymbol.Font != null && !characterMarkerSymbol.Color.IsTransparent())
                    return CreateEntitiesForCharacterMarker(characterMarkerSymbol, symbolAlignment, xOffset, yOffset);
            }
            else
                throw new NotSupportedException("Can only process MultiLayer-, Character- and Simple- Marker symbols.");

            return new EntityObjectInfo();
        }

        private EntityObjectInfo CreateEntitiesForMultiLayeMarker(IMultiLayerMarkerSymbol multiLayer, GlyphAlignment symbolAlignment, double xOffset, double yOffset)
        {
            var entities = new EntityObjectInfo();
            var layerVisibility = multiLayer as ILayerVisible;

            for (int i = multiLayer.LayerCount - 1; i >= 0; i--)
            {
                if (layerVisibility?.LayerVisible[i] ?? false)
                {
                    var symbolPartESRILayer = multiLayer.Layer[i];
                    entities += CreateEntitiesForMarkerSymbol(symbolPartESRILayer, symbolAlignment, xOffset, yOffset);
                }
            }

            return entities;
        }

        private EntityObjectInfo CreateEntitiesForCharacterMarker(ICharacterMarkerSymbol characterMarkerSymbol, GlyphAlignment glyphAlignment, double xOffset, double yOffset)
        {
            using (var glyph = GetGlyphForCharacterMarker(characterMarkerSymbol, glyphAlignment, xOffset, yOffset))
            {
                var fillColor = characterMarkerSymbol.Color.AciColor();
                var hatches = CreateHatches(glyph.HatchBoundaries, fillColor);

                return new EntityObjectInfo(hatches, glyph.Blackbox);
            }
        }

        /// <summary>
        /// Create dxf entities to represent the geometry of a glyph (symbol) of a font.
        /// </summary>
        /// <param name="characterMarkerSymbol"></param>
        /// <param name="glyphAlignment"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        private CGlyph GetGlyphForCharacterMarker(ICharacterMarkerSymbol characterMarkerSymbol, GlyphAlignment glyphAlignment, double xOffset, double yOffset)
        {
            return GetGlyphForMarker(characterMarkerSymbol.CharacterIndex, characterMarkerSymbol as IMarkerSymbol, glyphAlignment, xOffset, yOffset);
        }

        /// <summary>
        /// Extract glyph from font using windows gdi
        /// </summary>
        /// <param name="glyph"></param>
        /// <param name="markerSymbol"></param>
        /// <param name="glyphAlignment"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        private CGlyph GetGlyphForMarker(int glyph, IMarkerSymbol markerSymbol, GlyphAlignment glyphAlignment, double xOffset, double yOffset)
        {
            var cms = (markerSymbol as ISymbol);
            try
            {
                cms.SetupDC((int)_memDC, _displayTransformation);
                return new CGlyph(_memDC, (uint)glyph, markerSymbol.Size, glyphAlignment, xOffset, yOffset, markerSymbol.Angle);
            }
            finally
            {
                cms.ResetDC();
            }
        }

        /// <summary>
        /// create dxf entities to represent an arcGIS/ArcMap simple marker (IsimpleMarkerSymbol). These are predefined 
        /// (relatively) simple markers - like a cross, a poing (tiny circle), a rectangle (square), a diamond (45° rotated square),...
        /// </summary>
        /// <param name="simpleMarkerSymbol"></param>
        /// <param name="symbolAlignment"></param>
        /// <returns></returns>
        private EntityObjectInfo CreateEntitiesForSimpleMarker(ISimpleMarkerSymbol simpleMarkerSymbol, GlyphAlignment symbolAlignment)
        {
            switch (simpleMarkerSymbol.Style)
            {
                case esriSimpleMarkerStyle.esriSMSCircle:
                    return CreateFilledMarker(simpleMarkerSymbol, symbolAlignment, NewCircle);

                case esriSimpleMarkerStyle.esriSMSCross:
                    return CreateLineMarkers(simpleMarkerSymbol, symbolAlignment, NewCross, NewCrossOutline);

                case esriSimpleMarkerStyle.esriSMSDiamond:
                    return CreateFilledMarker(simpleMarkerSymbol, symbolAlignment, NewDiamond);

                case esriSimpleMarkerStyle.esriSMSSquare:
                    return CreateFilledMarker(simpleMarkerSymbol, symbolAlignment, NewRectangle);

                case esriSimpleMarkerStyle.esriSMSX:
                    return CreateLineMarkers(simpleMarkerSymbol, symbolAlignment, NewX, NewXOutline);

                default:
                    throw new NotSupportedException("Simple marker type other thatn circle, cross, diamond, square or x are not supported.");
            }
        }

        /// <summary>
        /// Create dxf entities to represent an arcGIS/ArcMap arrow marker (IArrowMarkerSymbol). This is a predefined simple borderless 
        /// triangle with user defined baseline (arcgis naming "widht") and height (arcgis naming "width").
        /// </summary>
        /// <param name="arrowMarkerSymbol"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        private EntityObjectInfo CreateEntitiesForArrowMarker(IArrowMarkerSymbol arrowMarkerSymbol, GlyphAlignment alignment)
        {
            return CreateArrowMarkerSymbol(arrowMarkerSymbol, alignment, NewTriangle);
        }

        /// <summary>
        /// Create filled marker symbol (consisting of a fill - hatch and an outline - in our special case also a hatch.
        /// Use dxf hatches so later scaling to real-world coordinates and resizing will work correctly
        /// (using i.e. lines wouls not resize line width when scaling up or down).
        /// </summary>
        /// <param name="simpleMarkerSymbol"></param>
        /// <returns></returns>
        private static EntityObjectInfo CreateFilledMarker(ISimpleMarkerSymbol symbol, GlyphAlignment alignment, AtomicGeometryCreator newGeometry)
        {
            return ((symbol.Outline) && (!symbol.OutlineColor.NullColor))
                ? CreateFilledMarkerWithBorder(symbol, alignment, newGeometry)
                : CreateBorderlessFilledMarker(symbol, alignment, newGeometry);
        }

        /// <summary>
        /// Create a - possibly - filled marker surrounded by a - visible - boundary/outline.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="alignment"></param>
        /// <param name="newGeometry"></param>
        /// <returns></returns>
        private static EntityObjectInfo CreateFilledMarkerWithBorder(ISimpleMarkerSymbol symbol, GlyphAlignment alignment, AtomicGeometryCreator newGeometry)
        {
            var baseSize = RawSymbolSize(symbol.Size);       // apply an empirically determined amount of leading 'round the symbol

            // The outline overlaps the fill by halfe of the outline width => the outline is centered on the fill boundary.
            var outlineSize = baseSize + symbol.OutlineSize;
            var fillSize = baseSize - symbol.OutlineSize;

            var position = AlignedPosition(symbol.XOffset, symbol.YOffset, alignment, symbol.Size);

            var bounds = new BoundingRectangle(position, symbol.Size, symbol.Size);

            var entities = new List<EntityObject>();

            var outline = new HatchBoundaryPath(new List<EntityObject>(1) { newGeometry(position, outlineSize) });

            var fill = new HatchBoundaryPath(new List<EntityObject>(1) { newGeometry(position, fillSize) });

            if (symbol.Color.NullColor)
                // The outline is the symbol, the inner - marker - is an island which shall not be drawn (left empty - transparent) at all.
                entities.Add(CreateHatch(new List<HatchBoundaryPath>(2) { outline, fill }, symbol.OutlineColor.AciColor()));
            else
            {
                entities.Add(CreateHatch(outline, symbol.OutlineColor.AciColor()));
                entities.Add(CreateHatch(fill, symbol.Color.AciColor()));
            }

            return new EntityObjectInfo(entities, bounds);
        }

        /// <summary>
        /// Create a borderless filled marker - a simple hatch without a visible boundary.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="alignment"></param>
        /// <param name="newGeometry"></param>
        /// <returns></returns>
        private static EntityObjectInfo CreateBorderlessFilledMarker(ISimpleMarkerSymbol symbol, GlyphAlignment alignment, AtomicGeometryCreator newGeometry)
        {
            var position = AlignedPosition(symbol.XOffset, symbol.YOffset, alignment, symbol.Size);

            var bounds = new BoundingRectangle(position, symbol.Size, symbol.Size);

            var fill = new HatchBoundaryPath(new List<EntityObject>(1) { newGeometry(position, RawSymbolSize(symbol.Size)) });

            return new EntityObjectInfo(CreateHatch(fill, symbol.Color.AciColor()), bounds);
        }

        /// <summary>
        /// Create marker consisting of a composed line symbol (cross, X, - two or more (simple) lines) possibly surrounded by an outline (made up of an underlying hatch heere).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="alignment"></param>
        /// <param name="newLineGeometry"></param>
        /// <param name="newOutlineGeometry"></param>
        /// <returns></returns>
        private static EntityObjectInfo CreateLineMarkers(ISimpleMarkerSymbol symbol, GlyphAlignment alignment, CompoundGeometryCreator newLineGeometry, AtomicGeometryCreator newOutlineGeometry)
        {
            return ((symbol.Outline) && (!symbol.OutlineColor.NullColor))
                ? CreateLineMarkerWithBorder(symbol, alignment, newLineGeometry, newOutlineGeometry)
                : CreateBorderlessLineMarker(symbol, alignment, newLineGeometry);
        }

        /// <summary>
        /// Create marker consisting of a composed line symbol (cross, X, - two or more (simple) lines) possibly surrounded by an outline (made up of an underlying hatch heere).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="alignment"></param>
        /// <param name="newLineGeometry"></param>
        /// <param name="newOutlineGeometry"></param>
        /// <returns></returns>
        private static EntityObjectInfo CreateLineMarkerWithBorder(ISimpleMarkerSymbol symbol, GlyphAlignment alignment, CompoundGeometryCreator newLineGeometry, AtomicGeometryCreator newOutlineGeometry)
        {
            var outlineSize = RawSymbolSize(symbol.Size);       // apply an empirically determined amount of leading 'round the symbol

            var fillSize = outlineSize - symbol.OutlineSize;    // where symbol.Outlinesize is the widht of the outline

            var position = AlignedPosition(symbol.XOffset, symbol.YOffset, alignment, symbol.Size);

            var bounds = new BoundingRectangle(position, symbol.Size, symbol.Size);

            var outline = newOutlineGeometry(position, outlineSize, symbol.OutlineSize);

            var entities = new List<EntityObject>();

            entities.Add(CreateHatch(outline, symbol.OutlineColor.AciColor()));

            if (!symbol.Color.NullColor)
            {
                var symbolColor = symbol.Color.AciColor();
                foreach (var f in newLineGeometry(position, symbol.Size))
                {
                    f.Color = symbolColor;
                    entities.Add(f);
                }
            }

            return new EntityObjectInfo(entities, bounds);
        }

        /// <summary>
        /// Create marker consisting of a composed line symbol (cross, X, - two or more (simple) lines).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="alignment"></param>
        /// <param name="newLineGeometry"></param>
        /// <param name="newOutlineGeometry"></param>
        /// <returns></returns>
        private static EntityObjectInfo CreateBorderlessLineMarker(ISimpleMarkerSymbol symbol, GlyphAlignment alignment, CompoundGeometryCreator newLineGeometry)
        {
            var position = AlignedPosition(symbol.XOffset, symbol.YOffset, alignment, symbol.Size);
            var bounds = new BoundingRectangle(position, symbol.Size, symbol.Size);
            var symbolColor = symbol.Color.AciColor();
            var entities = new List<EntityObject>();

            foreach (var f in newLineGeometry(position, RawSymbolSize(symbol.Size)))
            {
                f.Color = symbolColor;
                entities.Add(f);
            }

            return new EntityObjectInfo(entities, bounds);
        }

        /// <summary>
        /// Create marker consisting solely of a fill (no outline)
        /// </summary>
        /// <param name="symbol">IMarkerSymbol - mainly ArrowMarker</param>
        /// <param name="newGeometry">delegate to creaate the polyline forming the surrounding of the marker symbol</param>
        /// <returns></returns>
        private static EntityObjectInfo CreateArrowMarkerSymbol(IArrowMarkerSymbol symbol, GlyphAlignment alignment, AtomicGeometryCreator newGeometry)
        {
            if (!symbol.Color.NullColor)
            {
                var position = AlignedPosition(symbol.XOffset, symbol.YOffset, alignment, symbol.Length, symbol.Width);
                var fill = new HatchBoundaryPath(new List<EntityObject>(1) { newGeometry(position, symbol.Length, symbol.Width) });
                var bounds = new BoundingRectangle(position, symbol.Length, symbol.Width);

                return new EntityObjectInfo(CreateHatch(fill, symbol.Color.AciColor()), bounds);
            }
            else
                return new EntityObjectInfo();
        }

        private static double RawSymbolSize(double size) => size;
        private static EntityObject NewRectangle(Vector2 position, params double[] sizes)
        {
            var s = Decode2XY(sizes);

            var x = 0.5 * s.X;
            var y = 0.5 * s.Y;
            var vertexes = new List<Vector2>(4)
            { new Vector2(-x, -y) + position,
              new Vector2(+x, -y) + position,
              new Vector2(+x, +y) + position,
              new Vector2(-x, +y) + position
            };

            return new LwPolyline(vertexes) { IsClosed = true };
        }

        // In Fact the below is a 45 Deg rotated square
        private static EntityObject NewDiamond(Vector2 position, params double[] sizes)
        {
            var s = Decode2XY(sizes);
            s /= 2;

            var vertexes = new List<Vector2>(4)
            {
              new Vector2(-s.X, 0) + position,
              new Vector2(0, -s.Y) + position,
              new Vector2(+s.X, 0) + position,
              new Vector2(0, +s.Y) + position
            };

            return new LwPolyline(vertexes) { IsClosed = true };
        }

        private static EntityObject NewCircle(Vector2 position, params double[] sizes)
        {
            var s = Decode2XY(sizes);

            return new Circle(position, s.X / 2);
        }

        /// <summary>
        /// Create triangle 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sizes">size[0]: triangle baseline, size[1] triangle height</param>
        /// <returns></returns>
        private static EntityObject NewTriangle(Vector2 position, params double[] sizes)
        {
            var s = Decode2XY(sizes);

            var vertexes = new List<Vector2>(3)
            {
                new Vector2(-s.X/2, -s.Y/2) + position,
                new Vector2(s.X/2, 0) + position,
                new Vector2(-s.X/2, s.Y/2) + position
            };

            return new LwPolyline(vertexes) { IsClosed = true };
        }

        private static List<EntityObject> NewCross(Vector2 position, params double[] sizes)
        {
            return AssembleFigure(GetCrossArmsEndpoints(sizes[0]), position);
        }

        private static EntityObject NewCrossOutline(Vector2 position, params double[] sizes)
        {
            // offsets of the outline's shape points relative to the figure's end points.
            var outlineDxDy = sizes[1];
            // Endpoints ot the contour of the outline
            var armPoints = GetCrossArmsEndpoints(sizes[0]);

            var outlineOffsets = new List<Vector2>(12)
            {
                new Vector2(0, -outlineDxDy),
                new Vector2(-outlineDxDy, -outlineDxDy),
                new Vector2(-outlineDxDy, 0),
                new Vector2(+outlineDxDy, 0),
                new Vector2(+outlineDxDy, -outlineDxDy),
                new Vector2(0, -outlineDxDy),
                new Vector2(0, +outlineDxDy),
                new Vector2(+outlineDxDy, +outlineDxDy),
                new Vector2(+outlineDxDy, 0),
                new Vector2(-outlineDxDy, 0),
                new Vector2(-outlineDxDy, +outlineDxDy),
                new Vector2(0, +outlineDxDy)
            };

            return AssembleOutline(armPoints, outlineOffsets, position);
        }

        private static List<EntityObject> NewX(Vector2 position, params double[] sizes)
        {
            return AssembleFigure(GetXArmsEndpoints(sizes[0]), position);
        }

        private static Vector2 Decode2XY(params double[] sizes)
        {
            return new Vector2(sizes[0], (1 >= sizes.Count()) ? sizes[0] : sizes[1]);
        }

        private static Vector2 AlignedPosition(double x, double y, GlyphAlignment alignment, params double[] sizes)
        {
            return AlignedPosition(new Vector2(x, y), alignment, sizes);
        }

        private static Vector2 AlignedPosition(Vector2 position, GlyphAlignment alignment, params double[] sizes)
        {
            var s = Decode2XY(sizes);

            return position + AlignmentOffsetVector(alignment, s.X, s.Y);
        }

        private static Vector2 AlignmentOffsetVector(GlyphAlignment alignment, double x, double y)
        {
            switch (alignment)
            {
                case GlyphAlignment.topLeft: return new Vector2(LeftAlignedOffset(x), TopAlignedOffset(y));
                case GlyphAlignment.topCenter: return new Vector2(CenterAlignedOffset(x), TopAlignedOffset(y));
                case GlyphAlignment.topRight: return new Vector2(RightAlignedOffset(x), TopAlignedOffset(y));
                case GlyphAlignment.middleLeft: return new Vector2(LeftAlignedOffset(x), MiddleAlignedOffset(y));
                case GlyphAlignment.middleCenter: return new Vector2(CenterAlignedOffset(x), MiddleAlignedOffset(y));
                case GlyphAlignment.middleRight: return new Vector2(RightAlignedOffset(x), MiddleAlignedOffset(y));
                case GlyphAlignment.bottomLeft: return new Vector2(LeftAlignedOffset(x), BottomAlignedOffset(y));
                case GlyphAlignment.bottomCenter: return new Vector2(CenterAlignedOffset(x), BottomAlignedOffset(y));
                case GlyphAlignment.bottomRight: return new Vector2(RightAlignedOffset(x), BottomAlignedOffset(y));
                case GlyphAlignment.esriArrowMarker: return new Vector2(RightAlignedOffset(x), MiddleAlignedOffset(y));
                default: return new Vector2(0, 0);
            }
        }

        private static double LeftAlignedOffset(double width) => width / 2;
        private static double CenterAlignedOffset(double width) => 0;
        private static double RightAlignedOffset(double width) => -width / 2;
        private static double TopAlignedOffset(double height) => -height / 2;
        private static double MiddleAlignedOffset(double height) => 0;
        private static double BottomAlignedOffset(double height) => height / 2;

        private static EntityObject NewXOutline(Vector2 position, params double[] sizes)
        {
            var lwSqr = sizes[1] * sizes[1];
            var outlineDxDy = Math.Sqrt(lwSqr / 2.0);
            var centerOffset = Math.Sqrt(2 * lwSqr);

            // Endpoints ot the contour of the outline - the outline's arms are 45 deg rotated
            var armPoints = GetXArmsEndpoints(sizes[0]);

            var outlineOffsets = new List<Vector2>(12)
            {
                new Vector2(+outlineDxDy, -outlineDxDy),
                new Vector2(0, -centerOffset),
                new Vector2(-outlineDxDy, -outlineDxDy),
                new Vector2(+outlineDxDy, +outlineDxDy),
                new Vector2(+centerOffset, 0),
                new Vector2(+outlineDxDy, -outlineDxDy),
                new Vector2(-outlineDxDy, +outlineDxDy),
                new Vector2(0, +centerOffset),
                new Vector2(+outlineDxDy, +outlineDxDy),
                new Vector2(-outlineDxDy, -outlineDxDy),
                new Vector2(-centerOffset, 0),
                new Vector2(-outlineDxDy, +outlineDxDy),
            };

            return AssembleOutline(armPoints, outlineOffsets, position);
        }

        /// <summary>
        /// Get end points of the crosse's arms - as a list.
        /// => X - rotated 45 dec counter-clockwise (ccw)
        /// </summary>
        /// <param name="size"></param>
        /// <returns>
        /// [0] -> horizontal left, [1] horizontal right
        /// [3] -> vertical top, [3] vertical bottom
        /// </returns>
        private static List<Vector2> GetCrossArmsEndpoints(double size)
        {
            return new List<Vector2>(4)
            {
                new Vector2(-0.5 * size, 0),
                new Vector2(+0.5 * size, 0),
                new Vector2(0, -0.5 * size),
                new Vector2(0, +0.5 * size)
            };
        }

        /// <summary>
        /// Get end points of the X'es arms - as a list.
        /// => Cross - rotated 45 deg clockwise (cw)
        /// </summary>
        /// <returns>
        /// [0] -> top left, [1] bottom right
        /// [2] -> top right, [3] bottom left
        /// </returns>
        private static List<Vector2> GetXArmsEndpoints(double size)
        {
            return new List<Vector2>(4)
            {
                new Vector2(-0.5 * size, -0.5 * size),
                new Vector2(+0.5 * size, +0.5 * size),
                new Vector2(+0.5 * size, -0.5 * size),
                new Vector2(-0.5 * size, +0.5 * size)
            };
        }

        /// <summary>
        ///  simply assemble the figure - + or X - from the arm endpoints
        /// </summary>
        /// <param name="armPoints"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static List<EntityObject> AssembleFigure(List<Vector2> armPoints, Vector2 position)
        {
            return new List<EntityObject>(2)
            {
                new Line(armPoints[0] + position, armPoints[1] + position),
                new Line(armPoints[2] + position, armPoints[3] + position)
            };
        }

        /// <summary>
        /// Simply assemble the outline - this will be a hatch - so a sort of a more or less comples polygon 
        /// (formed by a closed polyline at first hand.
        /// </summary>
        /// <param name="armPoints">Points of the figure to wich the outline has to be assembled</param>
        /// <param name="outlineOffsets">offsets of the outline relative to the figure end points</param>
        /// <param name="position">Center position of the figure</param>
        /// <returns></returns>
        private static EntityObject AssembleOutline(List<Vector2> armPoints, List<Vector2> outlineOffsets, Vector2 position)
        {
            var bulge = Math.Tan(Math.PI / 4);  // create rounded shape

            var vertexes = new List<LwPolylineVertex>(12)
            {
                new LwPolylineVertex(position + armPoints[0] + outlineOffsets[0]),
                new LwPolylineVertex(position + outlineOffsets[1]),
                new LwPolylineVertex(position + armPoints[2] + outlineOffsets[2], bulge),
                new LwPolylineVertex(position + armPoints[2] + outlineOffsets[3]),
                new LwPolylineVertex(position + outlineOffsets[4]),
                new LwPolylineVertex(position + armPoints[1] + outlineOffsets[5], bulge),
                new LwPolylineVertex(position + armPoints[1] + outlineOffsets[6]),
                new LwPolylineVertex(position + outlineOffsets[7]),
                new LwPolylineVertex(position + armPoints[3] + outlineOffsets[8], bulge),
                new LwPolylineVertex(position + armPoints[3] + outlineOffsets[9]),
                new LwPolylineVertex(position + outlineOffsets[10]),
                new LwPolylineVertex(position + armPoints[0] + outlineOffsets[11], bulge)
            };

            return new LwPolyline(vertexes, true);
        }

        private static IEnumerable<Hatch> CreateHatches(IEnumerable<HatchBoundaryPath> hatchPaths, AciColor color)
        {
            if (0 < hatchPaths.Count())
                yield return CreateHatch(hatchPaths.ElementAt(0), hatchPaths.Skip(1), color);
        }

        private static Hatch CreateHatch(HatchBoundaryPath border, IEnumerable<HatchBoundaryPath> holes, AciColor color)
        {
            var boundaries = new List<HatchBoundaryPath>();
            boundaries.Add(border);

            foreach (var hole in holes)
                boundaries.Add(hole);

            return CreateHatch(boundaries, color);
        }

        private static Hatch CreateHatch(IEnumerable<HatchBoundaryPath> boundaries, AciColor color)
        {
            return new Hatch(HatchPattern.Solid, boundaries, false)
            {
                Color = color
            };
        }

        private static Hatch CreateHatch(HatchBoundaryPath simplePath, AciColor color)
        {
            return CreateHatch(new List<HatchBoundaryPath>(1) { simplePath }, color);
        }

        private static Hatch CreateHatch(IEnumerable<EntityObject> border, AciColor color)
        {
            var hbp = new HatchBoundaryPath(border);
            return CreateHatch(new List<HatchBoundaryPath>(1) { hbp }, color);
        }

        private static Hatch CreateHatch(EntityObject simpleClosedLoopObject, AciColor color)
        {
            return CreateHatch(new List<EntityObject>(1) { simpleClosedLoopObject }, color);
        }

   }
}

