using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.TTF;
using log4net;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class MarkerSymbology : Symbology<MarkerSymbolInfo>
    {
        protected Dictionary<SymbolWithAlignment, BlockInfo> _dxfBlocks = new Dictionary<SymbolWithAlignment, BlockInfo>();

        protected DxfBlockFactory _dxfBlockFactory;

        protected GlyphAlignment _symbolAlignment;
        public DxfBlockFactory DxfBlockFactory => _dxfBlockFactory;

        private static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        public MarkerSymbology(ILayer esriLayer, DxfBlockFactory dxfBlockFactory, double dotsToMeter, GlyphAlignment symbolAlignment)
            : base(esriLayer, dotsToMeter)
        {
            _dxfBlockFactory = dxfBlockFactory;
            _symbolAlignment = symbolAlignment;
        }

        public MarkerSymbology(ILayer esriLayer, DxfBlockFactory dxfBlockFactory, double dotsToMeter)
            : this(esriLayer, dxfBlockFactory, dotsToMeter, GlyphAlignment.middleCenter) 
        {
        }

        protected override MarkerSymbolInfo CreateInfoCore(IFeature feature, out ISymbol symbol)
        {
            var symbolInfo = base.CreateInfoCore(feature, out symbol);

            return symbolInfo;
        }

        protected override MarkerSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            return CreateInfoCore(symbol, _symbolAlignment);
        }
        
        protected override MarkerSymbolInfo CreateInfoCore(ISymbol symbol, GlyphAlignment alignment) 
        {
            var info = new MarkerSymbolInfo(symbol, _esriLayer);

            if (null != symbol)
            {
                info.Block = LookupBlock(symbol, alignment) ?? NewBlock(symbol, alignment);
            }

            return info;
        }

        #region private helpers

        private BlockInfo LookupBlock(ISymbol symbol, GlyphAlignment alignment)
        {
            if (_dxfBlocks.TryGetValue(new SymbolWithAlignment(symbol, alignment), out var searchedBlock))
                return searchedBlock;
            return null;
        }

        private BlockInfo NewBlock(ISymbol symbol, GlyphAlignment alignment)
        {
            try
            {
                var symbolName = SymbolName.Create();
                BlockInfo newBlock = _dxfBlockFactory.CreateBlock(symbolName, symbol, alignment);
                if (null != newBlock)
                {
                    _dxfBlocks.Add(new SymbolWithAlignment(symbol, alignment), newBlock);
                }
                return newBlock;
            }
            catch (NotSupportedException ex)
            {
                var msg = $"{ex.Message} at Layer: {_esriLayer.Name}";
                _log.Fatal(msg, ex);
                throw new NotSupportedException(msg, ex.InnerException);
            }
        }

        #endregion
    }
}
