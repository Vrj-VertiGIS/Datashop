using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.LineTypeFactory;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class LineSymbology : Symbology<LayeredLineSymbolInfo>
    {
        protected Dictionary<ILineSymbol, Linetype> _dxfLinetypes = null;
        protected IGeoFeatureLayer _geoFeatureLayer = null;

        protected MarkerSymbology _markerSymbology = null;

        public LineSymbology(ILayer esriLayer, MarkerSymbology markerSymbology, double dotsToMeter, IEqualityComparer<ILineSymbol> lineSymbolComparer = null)
            : base(esriLayer, dotsToMeter)
        {
            _geoFeatureLayer = esriLayer as IGeoFeatureLayer;
            _dxfLinetypes = new Dictionary<ILineSymbol, Linetype>(lineSymbolComparer);
            _markerSymbology = markerSymbology;
        }

        protected override LayeredLineSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            var info = new LayeredLineSymbolInfo();

            if (symbol is ILineSymbol ls)
                PopulateSymbolLayers(ls, info);

            return info;
        }

        #region private helpers

        protected void PopulateSymbolLayers(ILineSymbol symbol, LayeredLineSymbolInfo info)
        {
            if (symbol is IMultiLayerLineSymbol mls)
            {
                for (var i = 0; (mls.LayerCount > i); i++)
                    PopulateSymbolLayer(mls, info, i);
            }
            else
                PopulateSymbolLayer(symbol, info);
        }

        private void PopulateSymbolLayer(IMultiLayerLineSymbol symbolLayers, LayeredLineSymbolInfo info, int layerIndex)
        {
            if (((ILayerVisible)symbolLayers).LayerVisible[layerIndex])
                PopulateSymbolLayer(symbolLayers.Layer[layerIndex], info);
        }

        private void PopulateSymbolLayer(ILineSymbol symbol, LayeredLineSymbolInfo info)
        {
            var decoration = GetLineDecoration(symbol);

            if (symbol is IMarkerLineSymbol)
                // Line made up of a sequence of marker symbols
                info.Add(new MarkerLineSymbolInfo(symbol as ISymbol, _esriLayer, _markerSymbology, decoration));
            else
                // Line defined by a dxf linestyle - usually coninuous or a simple dash/dot combination
                info.Add(new LinetypeLineSymbolInfo(symbol as ISymbol, _esriLayer, GetLinetype(symbol), decoration));
        }

        private Linetype GetLinetype(ILineSymbol symbol)
        {
            var linetype = LookupLinetype(symbol);
            if (null == linetype)
                linetype = NewLinetype(symbol, SymbolName.Create());

            return linetype;
        }

        private Linetype LookupLinetype(ILineSymbol symbol)
        {
            Linetype searchedLinetype;

            if (_dxfLinetypes.TryGetValue(symbol, out searchedLinetype))
                return searchedLinetype;
            else
                return null;
        }

        private Linetype NewLinetype(ILineSymbol symbol, string symbolName)
        {
            Linetype lineType = null;

            if (symbol is ILineProperties lprops)
                lineType = new LineByPropertiesLineTypeFactory(symbolName, symbol, _dotsToMeter).ToLine();
            else if (symbol is ISimpleLineSymbol sls)
                lineType = new SimpleLineTypeFactory(symbolName, sls, _dotsToMeter).ToLine();
            else
                lineType = Linetype.Continuous.Clone() as Linetype;

            if (null != lineType)
                _dxfLinetypes.Add(symbol, lineType);

            return lineType;
        }

        private LineDecorations GetLineDecoration(ILineSymbol symbol)
        {
            return new LineDecorations(symbol as ILineProperties, _markerSymbology);
        }

        #endregion
    }
}