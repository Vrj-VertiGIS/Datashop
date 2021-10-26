using System.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class MarkerSymbolInfo : SymbolInfo
    {
        private BlockInfo _blockInfo = null;
        private double _symbolRotation = 0.0;
        private IMarkerSymbol _markerSymbol = null;

        public MarkerSymbolInfo()               // Keep C# happy
            : base()
        {
        }

        public MarkerSymbolInfo(ISymbol symbol, ILayer layer)
            : base(symbol, layer)
        {
            _markerSymbol = symbol as IMarkerSymbol;
            _symbolRotation = (null != _markerSymbol)
                ? MathSnippets.NormalizeDeg(_markerSymbol.Angle)
                : 0.0;
        }

        public BlockInfo Block
        {
            get { return _blockInfo; }
            set { _blockInfo = value; }
        }

        public override bool IsVisible
        {
            get
            {
                var visible = (_blockInfo != null) && (0 < _blockInfo.Block.Entities.Count) && base.IsVisible;
                return (_markerSymbol is ISimpleMarkerSymbol sms)
                    ? (visible && sms.Outline)
                    : visible;
            }
        }

        public override byte Opacity
        {
            get
            {
                if ((_markerSymbol is IMultiLayerMarkerSymbol mls) && (0 < mls.LayerCount))
                    return mls.LayersAsEnumerable().Max(s => SymbolLayerTransparency(s)); // Maximum opacity value 
                else if (_markerSymbol is ISimpleMarkerSymbol sms)
                    return (sms.Color.Transparency > sms.OutlineColor.Transparency) ? sms.Color.Transparency : sms.OutlineColor.Transparency;
                else if (null != _markerSymbol)
                    return _markerSymbol.Color.Transparency;
                else
                    return 255;
            }
        }

        private static byte SymbolLayerTransparency(IMarkerSymbol ms)
        => (ms is ISimpleMarkerSymbol sms)
            ? MaxOf(sms.Color.Transparency, sms.Outline ? sms.OutlineColor.Transparency : (byte)0)
            : ms.Color.Transparency;

        private static byte MaxOf(params byte[] bytes)
            => bytes.Max();

        public double Rotation
        {
            get => _symbolRotation;
            set => _symbolRotation = MathSnippets.NormalizeDeg(value);
        }

        public double SizeInDots => (null != _markerSymbol) ? _markerSymbol.Size : 0.0;

    }
}
