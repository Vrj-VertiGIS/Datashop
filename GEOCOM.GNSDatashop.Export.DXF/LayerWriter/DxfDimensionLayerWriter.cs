using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    public class DxfDimensionLayerWriter : DxfLayerWriterBase
    {
        protected DimensionSymbology _symbology = null;

        protected IDimensionClassExtension _dimensionClassExtension = null;

        private IDimensionGraphic _dimensionGraphic;

        private ExpressionParsers _expressionParsers;

        internal DxfDimensionLayerWriter(_IDxfWriterContext context, IFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer, ExpressionParsers expressionParsers)
            : base(context, esriLyr, features, dxfLayer)
        {
            _dimensionClassExtension = (_esriLyr as IFeatureLayer)?.FeatureClass?.Extension as IDimensionClassExtension;
            var arrowMarkerSymbology = new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter, TTF.GlyphAlignment.esriArrowMarker);
            var linePatternSymbology = new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter, TTF.GlyphAlignment.middleCenter);
            _symbology = new DimensionSymbology
                (
                _esriLyr,
                arrowMarkerSymbology,
                new LineSymbology(_esriLyr, linePatternSymbology, DotsToMeter),
                new DimensionTextSymbology(_esriLyr, _context.DxfDocument.TextStyles, arrowMarkerSymbology, DotsToMeter),
                DotsToMeter
                );

            _dimensionGraphic = new DimensionGraphicClass() as IDimensionGraphic;
            _expressionParsers = expressionParsers;
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var dimFeature = feature as IDimensionFeature;
            var shape = (dimFeature)?.DimensionShape as IDimensionShape;

            if (null != shape)
            {
                var symbolInfo = _symbology.CreateInfo(feature);
                try
                {
                    WriteFeatureCore(dimFeature, symbolInfo);
                }
                finally
                {
                    symbolInfo.Dispose();
                }

                base.WriteFeature(feature);
            }
        }

        private void WriteFeatureCore(IDimensionFeature dimFeature, MapSymbology.Info.DimensionSymbolInfo symbolInfo)
        {
            PrepareDimensionDisplay(dimFeature, symbolInfo.Style);
            var figure = new DimensionFigure(dimFeature, _dimensionGraphic, symbolInfo, this, _expressionParsers);

            WriteEntity(figure.DxfEntities);
        }

        private void PrepareDimensionDisplay(IDimensionFeature feature, IDimensionStyle style)
        {
            _dimensionGraphic.Style = style;
            _dimensionGraphic.DimensionShape = feature.DimensionShape;
            _dimensionGraphic.UseCustomLength = feature.UseCustomLength;
            _dimensionGraphic.CustomLength = feature.CustomLength;
            _dimensionGraphic.NativeUnits = _dimensionClassExtension.ReferenceScaleUnits;
            _dimensionGraphic.NativeTransformation = (_dimensionClassExtension as IDimensionClassExtension2).NativeTransformation[_dimensionGraphic];
            _dimensionGraphic.UpdateShape(HDC.ToInt32(), RenderDisplay.DisplayTransformation, feature as IFeature);
        }

        internal override double DotsToMeterScaled => 25.4 / 72000.0 * _dimensionClassExtension.ReferenceScale;

        public override double DotsToMeter => ((null != _dimensionClassExtension) && (0 < _dimensionClassExtension.ReferenceScale))
            ? DotsToMeterScaled
            : DotsToMeterUnscaled;
    }
}
