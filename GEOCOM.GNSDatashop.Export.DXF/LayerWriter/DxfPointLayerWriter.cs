using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    internal class DxfPointLayerWriter : DxfLayerWriterBase
    {
        protected MarkerSymbology _symbology = null;

        internal DxfPointLayerWriter(_IDxfWriterContext context, IGeoFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context, esriLyr, features, dxfLayer)
        {
            _symbology = new MarkerSymbology(esriLyr, BlockFactory, DotsToMeter);
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var ptClipped = Clip(feature.Shape);

            if ((null != ptClipped) && (!ptClipped.IsEmpty))
            {
                var symbolInfo = _symbology.CreateInfo(feature);

                if (symbolInfo.IsVisible)
                {
                    var insert = EntityFactory.CreateBlockInsert(symbolInfo.Block.Block, symbolInfo.Rotation, ptClipped as IPoint, symbolInfo);

                    WriteEntity(insert);

                    base.WriteFeature(feature);
                }
            }
        }


    }
}
