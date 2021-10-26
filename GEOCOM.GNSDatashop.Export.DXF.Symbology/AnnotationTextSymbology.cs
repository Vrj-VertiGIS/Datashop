using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.Common.Clipping;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement;
using GEOCOM.GNSDatashop.TTF;
using netDxf.Collections;
using System;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class AnnotationTextSymbology : TextSymbologyBase<AnnotationTextSymbolInfo>
    {
        private IAnnotationFeature _annoFeature;

        private int _fld_Angle;

        public RegionOfInterest RegionOfInterest { get; set; } = null;

        public AnnotationTextSymbology(ILayer esriLayer, TextStyles textStyles, MarkerSymbology markerSymbology, double dotsToMeter)
            : base(esriLayer,  dotsToMeter, textStyles, markerSymbology)
        {
            _fld_Angle = esriLayer is ILayerFields fields ? fields.FindQualifiedField("Angle") : -1;
        }

        public AnnotationTextSymbolInfo CreateInfo(IElement element)
        {
            return CreateInfoCore(element, out var symbol);
        }

        protected override AnnotationTextSymbolInfo CreateInfoCore(IFeature feature, out ISymbol symbol)
        {
            _annoFeature = (IAnnotationFeature) feature;

            return CreateInfoCore(_annoFeature.Annotation, out symbol);
        }

        private AnnotationTextSymbolInfo CreateInfoCore(IElement element, out ISymbol symbol)
        {
            // Create an AnnotationTextSymbolInfo without a TextElement. If we find a TextElement in "element" we will create a new one.
            var symbolInfo = new AnnotationTextSymbolInfo(null, _esriLayer);
            CreateInfoCore(element, ref symbolInfo);
            symbol = symbolInfo.Symbol;
            return symbolInfo;
        }

        private void CreateInfoCore(IElement element, ref AnnotationTextSymbolInfo symbolInfo)
        {
            if (element is IGroupElement groupElement)
            {
                IEnumElement elements = groupElement.Elements;
                IElement thisElement;
                while (null != (thisElement = elements.Next()))
                    CreateInfoCore(thisElement, ref symbolInfo);
            }
            else 
                ProcessElementIfSupported(element, ref symbolInfo);
        }

        private void ProcessElementIfSupported(IElement element, ref AnnotationTextSymbolInfo symbolInfo)
        {
            if (element is ISymbolCollectionElement symbolElement)
            {
                // Only text elements implement ISymbolCollectionElement
                // Create a new symbolInfo if we do not yet have a symbolInfo that is based on a text element.
                if (null == symbolInfo.TextSymbol)
                    symbolInfo.SymbolCollectionElement = symbolElement;
                ProcessTextElement(symbolElement, symbolInfo);
            }
            else if (element is IMarkerElement markerElement)
                ProcessMarkerElement(markerElement, symbolInfo);
            else if (element is ILineElement lineElement)
                ProcessLineElement(lineElement, symbolInfo);
            else if (element is IFillShapeElement fillShapeElement)
                ProcessFillShapeElement(fillShapeElement, symbolInfo);
            else
                Logger.Error($"Unsupported element: {element.GetType()}");
        }

        protected override AnnotationTextSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            return new AnnotationTextSymbolInfo();
        }

        private void ProcessTextElement(ISymbolCollectionElement symbolElement, AnnotationTextSymbolInfo symbolInfo)
        {
            symbolInfo.TextStyle = GetTextStyle(symbolInfo);

            LabelPlacement placement = LabelPlacement.Create(symbolInfo);
            placement?.Apply(symbolInfo);

            Set_TextAngle(symbolInfo);

            // var pos = symbolElement.TextPath.Positions().ToList(); TBD: Export callouts

            var clipped = Clip(((IElement)symbolElement).Geometry);
            symbolInfo.AddGeometryAndSymbol(clipped, symbolInfo);
        }

        private void ProcessLineElement(ILineElement lineElement, AnnotationTextSymbolInfo symbolInfo)
        {
            ProcessLineElement(lineElement as IElement, lineElement.Symbol, symbolInfo);
            LayeredLineSymbolInfo leaderLineSymbolInfo = _leaderLineSymbology.CreateInfo(lineElement.Symbol as ISymbol);
            var clipped = Clip(((IElement)lineElement).Geometry);
            symbolInfo.AddGeometryAndSymbol(clipped, leaderLineSymbolInfo);
        }

        private void ProcessLineElement(IElement element, ILineSymbol lineSymbol, AnnotationTextSymbolInfo symbolInfo)
        {
            LayeredLineSymbolInfo leaderLineSymbolInfo = _leaderLineSymbology.CreateInfo(lineSymbol as ISymbol);
            var clipped = Clip(element.Geometry);
            symbolInfo.AddGeometryAndSymbol(clipped, leaderLineSymbolInfo);
        }

        private void ProcessFillShapeElement(IFillShapeElement fillShapeElement, AnnotationTextSymbolInfo symbolInfo)
        {
            LayeredFillSymbolInfo layeredFillSymbolInfo = _fillSymbology.CreateInfo(fillShapeElement.Symbol as ISymbol);
            var clipped = Clip(((IElement)fillShapeElement).Geometry);
            symbolInfo.AddGeometryAndSymbol(clipped, layeredFillSymbolInfo);
        }

        private void ProcessMarkerElement(IMarkerElement markerElement, AnnotationTextSymbolInfo symbolInfo)
        {
            GlyphAlignment glyphAlignment = (markerElement is IElementProperties3 properties) ? FromEsri(properties.AnchorPoint, true) : GlyphAlignment.middleCenterFull;
            var markerSymbolInfo = _anchorPointSymbology.CreateInfo(markerElement.Symbol as ISymbol, glyphAlignment);
            markerSymbolInfo.Rotation = markerElement.Symbol.Angle;
            var clipped = Clip(((IElement)markerElement).Geometry);
            symbolInfo.AddGeometryAndSymbol(clipped, markerSymbolInfo);
        }

        private void Set_TextAngle(AnnotationTextSymbolInfo symbolInfo)
        {
            if ((0 <= _fld_Angle) && (_annoFeature is IFeature feature))
                Set_TextAngle(symbolInfo, feature.Value[_fld_Angle]);
        }

        private void Set_TextAngle(AnnotationTextSymbolInfo symbolInfo, object value)
        {
            if ((null != value) && (DBNull.Value != value))
                try
                {
                    symbolInfo.Set_Angle((double)value);
                }
                catch (Exception)
                {
                }
        }

        #region private helpers

        /// <summary>
        /// Clip 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal IGeometry Clip(IGeometry geometry)
            => (null != geometry) && (!geometry.IsEmpty) && (null != RegionOfInterest)
                ? RegionOfInterest.Clip(geometry)
                : geometry;

        private GlyphAlignment FromEsri(esriAnchorPointEnum anchorPoint, bool full)
        {
            switch (anchorPoint)
            {
                case esriAnchorPointEnum.esriTopLeftCorner:
                    return GlyphAlignment.topLeft;
                case esriAnchorPointEnum.esriTopMidPoint:
                    return GlyphAlignment.topCenter;
                case esriAnchorPointEnum.esriTopRightCorner:
                    return GlyphAlignment.topRight;

                case esriAnchorPointEnum.esriLeftMidPoint:
                    return full ? GlyphAlignment.middleLeftFull : GlyphAlignment.middleLeft;
                case esriAnchorPointEnum.esriCenterPoint:
                    return full ? GlyphAlignment.middleCenterFull : GlyphAlignment.middleCenter;
                case esriAnchorPointEnum.esriRightMidPoint:
                    return full ? GlyphAlignment.middleRightFull : GlyphAlignment.middleRight;

                case esriAnchorPointEnum.esriBottomLeftCorner:
                    return GlyphAlignment.bottomLeft;
                case esriAnchorPointEnum.esriBottomMidPoint:
                    return GlyphAlignment.bottomCenter;
                case esriAnchorPointEnum.esriBottomRightCorner:
                    return GlyphAlignment.bottomRight;

                default:
                    return full ? GlyphAlignment.middleCenterFull : GlyphAlignment.middleCenter;
            }
        }

        #endregion

    }
}
