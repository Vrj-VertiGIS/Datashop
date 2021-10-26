using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using netDxf.Tables;
using System;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    internal class DxfEsriDimensionLayerWriter : DxfLayerWriterBase
    {
        protected DimensionSymbology _symbology = null;

        protected IDimensionClassExtension _dimensionClassExtension = null;

        internal DxfEsriDimensionLayerWriter(_IDxfWriterContext context, IFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context, esriLyr, features, dxfLayer)
        {
            _dimensionClassExtension = (_esriLyr as IFeatureLayer)?.FeatureClass?.Extension as IDimensionClassExtension;
            var markerSymbology = new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter, TTF.GlyphAlignment.middleLeft);
            _symbology = new DimensionSymbology
                (
                _esriLyr,
                markerSymbology,
                new LineSymbology(_esriLyr, markerSymbology, DotsToMeter),
                new DimensionTextSymbology(_esriLyr, _context.DxfDocument.TextStyles, markerSymbology, DotsToMeter),
                DotsToMeter
                );
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var symbolInfo = _symbology.CreateInfo(feature);
            var shape = (feature as IDimensionFeature)?.DimensionShape as IDimensionShape;

            if (null != shape)
            {
                var dimensionLineVector = shape.EndDimensionPoint.Subtract(shape.BeginDimensionPoint);
                var dimensionLineBeginPoint = shape.DimensionLinePoint;
                var dimensionLineEndPoint = shape.DimensionLinePoint.Add(dimensionLineVector);

                WriteDimensionLine(dimensionLineBeginPoint, dimensionLineEndPoint, shape.TextPoint, symbolInfo);
                if (null != symbolInfo.BeginExtensionLine)
                    WriteExtensionLine(shape.BeginDimensionPoint, dimensionLineBeginPoint, symbolInfo.ExtensionLineOvershot, symbolInfo.BeginExtensionLine);
                if (null != symbolInfo.EndExtensionLine)
                    WriteExtensionLine(shape.EndDimensionPoint, dimensionLineEndPoint, symbolInfo.ExtensionLineOvershot, symbolInfo.EndExtensionLine);

                if (null != symbolInfo.Text) 
                    WriteText(shape.TextPoint, shape.TextAngle, symbolInfo.Text);
            }
        }

        private void WriteDimensionLine(IPoint begin, IPoint end, IPoint textPoint, DimensionSymbolInfo symbolInfo)
        {
            if (null != symbolInfo.BeginDimensionLine)
                WriteDimLine(begin, end, symbolInfo.BeginDimensionLine);

            //if (null != symbolInfo.EndDimensionLine)
            //    WriteDimLine(textPoint, end, symbolInfo.EndDimensionLine);

            if (null != symbolInfo.BeginMarker)
                WriteDimensionMarker(begin, symbolInfo.BeginMarker);

            if (null != symbolInfo.EndMarker)
                WriteDimensionMarker(end, symbolInfo.EndMarker);
        }

        private void WriteExtensionLine(IPoint footPoint, IPoint topPoint, double overshoot, LayeredLineSymbolInfo symbolInfo)
        {
            if (Math.Abs(overshoot) > 1E-6)
                topPoint = topPoint.Add(topPoint.Subtract(footPoint).Offset(overshoot));

            WriteDimLine(footPoint, topPoint, symbolInfo);
        }

        private void WriteDimensionMarker(IPoint point, MarkerSymbolInfo symbolInfo)
        {
            var insert = EntityFactory.CreateBlockInsert(symbolInfo.Block.Block, symbolInfo.Rotation, point, symbolInfo);

            WriteEntity(insert);
        }

        private void WriteText(IPoint textPoint, double textAngle, TextSymbolInfo symbolInfo)
        {
            WriteEntity(EntityFactory.CreateMText(symbolInfo));
        }

        private void WriteDimLine(IPoint begin, IPoint end, LayeredLineSymbolInfo symbolInfo)
        {
            var line = new PolylineClass() { FromPoint = begin, ToPoint = end };
            var entities = EntityFactory.CreatePolyline(line, symbolInfo);
            WriteEntity(entities);
        }

        internal override double DotsToMeterScaled => 25.4 / 72000.0 * _dimensionClassExtension.ReferenceScale;

        internal override double DotsToMeter => ((null != _dimensionClassExtension) && (0 < _dimensionClassExtension.ReferenceScale))
            ? DotsToMeterScaled
            : DotsToMeterUnscaled;
    }

}
