using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using netDxf.Tables;
#if DEBUG
using System.Diagnostics;
#endif

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    internal class DxfPolylineLayerWriter : DxfLayerWriterBase
    {
        protected LineSymbology _symbology = null;

        internal DxfPolylineLayerWriter(_IDxfWriterContext context, IGeoFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context, esriLyr, features, dxfLayer)
        {
            // No need to compare symbols by data (see text-/fillsymbology). When line symbols are kept in the map toc, they have 
            // an ensured identity by object.
            _symbology = new LineSymbology(_esriLyr,
                new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter),
                DotsToMeter);
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var symbolInfo = _symbology.CreateInfo(feature);

            var polyline = Clip(feature.Shape);

            if ((null != polyline) && (!polyline.IsEmpty))
            {
                var entityLayers = EntityFactory.CreatePolylineGeometries(symbolInfo, polyline as IPolyline);

                foreach (var entityLayer in entityLayers)
                    WriteEntity(entityLayer);

                base.WriteFeature(feature);
            }
        }

    }
}




