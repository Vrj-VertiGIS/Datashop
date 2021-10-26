using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement;
using GEOCOM.GNSDatashop.Export.DXF.TextRendererAggregate;
using netDxf.Collections;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    /// <summary>
    /// Handling GEOCOM unuque value textrenderer symbology (labeling)
    /// </summary>
    public class GEOCOMUVTSymbology : TextSymbologyBase<GEOCOMUVTSymbolInfo>
    {
        private IUVTRenderer _textRenderer = null;

        private IFeature _feature = null;

        private int _labelFieldIndex = -1;
        private int _alignFieldIndex = -1;

        private IDictionary<ISymbol, FakeSimpleMarker> _fakeMarkers = new Dictionary<ISymbol, FakeSimpleMarker>(new FakeSymbolComparer());
        public GEOCOMUVTSymbology(IGeoFeatureLayer esriLayer, double dotsToMeter, TextStyles textStyles, MarkerSymbology markerSymbology)
            : base(esriLayer, dotsToMeter, textStyles, markerSymbology)
        {
            _textRenderer = new GEOCOM.GNSDatashop.Export.DXF.TextRendererAggregate.TextRendererAggregate(esriLayer.Renderer) as IUVTRenderer;

            LookupFieldIndices(esriLayer as ILayerFields, _textRenderer);
        }

        private void LookupFieldIndices(ILayerFields fields, IUVTRenderer renderer)
        {
            var labelField = renderer.TextField;
            _labelFieldIndex = (!string.IsNullOrEmpty(labelField)) ? fields.FindField(labelField) : -1;
            var alignField = renderer.AlignField;
            _alignFieldIndex = (!string.IsNullOrEmpty(alignField)) ? fields.FindField(alignField) : -1;
        }

        protected override GEOCOMUVTSymbolInfo CreateInfoCore(IFeature feature, out ISymbol symbol)
        {
            _feature = feature; // Keep this for later extraction of the real text info (which has not been programmed correctly in the renderer)

            var textRenderer1 = _textRenderer as IUVTRenderer1;
            if (null == textRenderer1)
                return base.CreateInfoCore(feature, out symbol);    // Ordinary, non-custom text symbol
            else
            {
                symbol = null;                                      // GEOCOM custom text renderer symbol
                ISymbol pointerLineSymbol = null;
                textRenderer1.getSymbolsByFeature(_feature, ref symbol, ref pointerLineSymbol);
                if (null != symbol)
                {
                    var symbolInfo = CreateInfoCore(symbol);        // Create text symbol info and apply text symbol 
                    symbolInfo.PointerLineSymbol = pointerLineSymbol as ILineSymbol;

                    if ((textRenderer1.ShowAnchorPoint) 
                        && (symbolInfo.IsVisible)
                        && (null != textRenderer1.AnchorPointSymbol) 
                        && (textRenderer1.AnchorPointSymbol is IMarkerSymbol mrs))
                    {
                        // var anchorPointSymbolInfo = _anchorPointSymbology.CreateInfo(mrs as ISymbol);
                        // Replaced this by a fake marker at the geonis gext renderer will not supply a suitable
                        // anchor point symbol when ran in batch mode.
                        var anchorPointSymbol = GetFakeMarker(symbol);
                        var anchorPointSymbolInfo = _anchorPointSymbology.CreateInfo(anchorPointSymbol as ISymbol);
                        var anchorPointGeometry = new ESRI.ArcGIS.Geometry.Point();
                        anchorPointGeometry.PutCoords(symbolInfo.ReferencePoint.X, symbolInfo.ReferencePoint.Y);
                        symbolInfo.AddGeometryAndSymbol(anchorPointGeometry, anchorPointSymbolInfo);
                    }

                    return symbolInfo;
                }

                return new GEOCOMUVTSymbolInfo();
            }
        }

        protected override GEOCOMUVTSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            var info = new GEOCOMUVTSymbolInfo(symbol, _esriLayer); // Pass the text symbol heere

            info.TextStyle = GetTextStyle(info);

            if (0 <= _labelFieldIndex)
                info.Set_Text(_feature.ValueAsString(_labelFieldIndex));

            PlaceSymbol(info);

            return info;
        }

        private void PlaceSymbol(GEOCOMUVTSymbolInfo info)
        {
            var geometry = _feature.Shape as IGeometry;
            if ((null != geometry) && (!geometry.IsEmpty))
            {
                var alignement = GetFeatureAlignement(info.Alignment);
                var placement = LabelPlacement.Create(geometry, alignement);
                if (null != placement)
                    placement.Apply(info);
            }
        }

        private HVAlignment GetFeatureAlignement(HVAlignment defaultAlignement)
        {
            if (0 <= _alignFieldIndex)
            {
                var alignementAsByGEOCOM = _feature.ValueAsString(_alignFieldIndex);
                HVAlignment alignment;
                if (!HVAlignments.TryGetAlignment(alignementAsByGEOCOM, out alignment))
                    alignment = defaultAlignement;   // Keep symbols default alignement
                return alignment;
            }
            else
                return defaultAlignement;
        }

        /// <summary>
        /// Get a fake simple marker (as the "real" one cannot be retrieved from the text renderer).
        /// Keep a dictionary to avoid creating a new one (and overflooding the DXF BLOCK_RECORDs table)
        /// for every single anchor point we want to convert to dxf.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private FakeSimpleMarker GetFakeMarker(ISymbol symbol)
        {
            if (!_fakeMarkers.TryGetValue(symbol, out FakeSimpleMarker _marker))
            {
                _marker = new FakeSimpleMarker(symbol);
                _fakeMarkers.Add(symbol, _marker);
            }
            return _marker;
        }

        private class FakeSymbolComparer : IEqualityComparer<ISymbol>
        {
            public bool Equals(ISymbol x, ISymbol y)
            {
                if ((x is ITextSymbol tsX) && (y is ITextSymbol tsy))
                    return (tsX.Size == tsy.Size)
                        && (tsX.Color.RGB == tsy.Color.RGB);
                else
                    return false;
            }

            public int GetHashCode(ISymbol obj)
            {
                var ts = obj as ITextSymbol;
                return (ts?.Color?.RGB.GetHashCode() ?? 0)
                    + (10 * ts?.Size.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Fake a anchor point marker symbol in case this cannot be reliably queried from the
        /// geonis text renderer (in batch mode).
        /// Attn: Fake only - not all properties are fully implemented!!
        /// </summary>
        private class FakeSimpleMarker : ISymbol, IMarkerSymbol, ISimpleMarkerSymbol, IClone
        {
            private IColor _markerColor = null;
            private double _textSymbolSize = 0.0;
            private double _angle = 0.0;

            /// <summary>
            /// Create a fake marker used to represent an anchor point symbol from
            /// the given text symbol.
            /// </summary>
            /// <param name="symbol"></param>
            public FakeSimpleMarker(ISymbol symbol)
            {
                var ts = symbol as ITextSymbol;
                _markerColor = ts?.Color ?? new RgbColor { RGB = 0x000000 };
                _textSymbolSize = ts?.Size ?? 8.0;
            }

            private FakeSimpleMarker() { }

            public void SetupDC(int hDC, ITransformation Transformation) { }

            public void ResetDC() { }

            public void Draw(IGeometry Geometry) { }

            public void QueryBoundary(int hDC, ITransformation displayTransform, IGeometry Geometry, IPolygon boundary) { }

            public esriRasterOpCode ROP2 
            { get => esriRasterOpCode.esriROPCopyPen; set => throw new System.NotImplementedException(); }
            public double Size 
            { get => _textSymbolSize / 8; set => throw new System.NotImplementedException(); }
            public IColor Color 
            { get => new RgbColor { NullColor = true }; set => throw new System.NotImplementedException(); }
            public double Angle 
            { get => _angle; set => _angle = value; }
            public double XOffset 
            { get => 0.0; set => throw new System.NotImplementedException(); }
            public double YOffset 
            { get => 0.0; set => throw new System.NotImplementedException(); }
            public esriSimpleMarkerStyle Style 
            { get => esriSimpleMarkerStyle.esriSMSCircle; set => throw new System.NotImplementedException(); }
            public bool Outline 
            { get => true; set => throw new System.NotImplementedException(); }
            public double OutlineSize 
            { get => _textSymbolSize / 20; set => throw new System.NotImplementedException(); }

            public IColor OutlineColor 
            { get => _markerColor; set => throw new System.NotImplementedException(); }

            public IClone Clone()
            {
                var cln = new FakeSimpleMarker();
                cln._markerColor = this._markerColor;
                cln._textSymbolSize = this._textSymbolSize;
                return cln as IClone;
            }

            public void Assign(IClone src)
            {
                var other = src as FakeSimpleMarker;
                if (null != other)
                {
                    this._markerColor = other._markerColor;
                    this._textSymbolSize = other._textSymbolSize;
                }
            }

            public bool IsEqual(IClone other)
                => IsIdentical(other) 
                    || ((other is FakeSimpleMarker fsm)
                        && (fsm._textSymbolSize == this._textSymbolSize)
                        && (fsm._markerColor == this._markerColor));

            public bool IsIdentical(IClone other)
                => this == other;
        }
    }
}
