using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;

using GEOCOM.GNSDatashop.Export.DXF.Common;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    /// <summary>
    /// Wrapping and handling of symbols (esri symbols - e.g. marker symbol, text symbol,...) 
    /// </summary>
    public abstract class SymbolInfo : ISymbolInfo
    {
        protected ISymbol _symbol = null;
        protected ILayer _esriLayer = null;

        public SymbolInfo()
        {
        }

        protected SymbolInfo(ISymbol symbol, ILayer layer)
        {
            _esriLayer = layer;
            Symbol = symbol;
        }

        public virtual ISymbol Symbol
        {
            get => _symbol;
            protected set => _symbol = value;
        }

        public ILayer EsriLayer => _esriLayer;

        // Transparency of layer (alpha value) - [0..255] - 255 = fully transparent
        protected byte LayerTransparency => (null != _esriLayer) && (_esriLayer is ILayerEffects le) 
            ? (byte) (255*le.Transparency / 100) 
            : (byte)0;

        // Opacity of layer (alpha value) - [0..255] - 255 = fully opaque
        protected byte LayerOpacity => (byte) (255 - LayerTransparency);

        /// <summary>
        /// Override in descendants: Return the opacity (0=transparent,...,255=fully opaque) of the symbol.
        /// </summary>
        public abstract byte Opacity { get; }  // Overall opacity (alpha value) [0 .. 255] // 255 = fully opaque

        /// <summary>
        /// The "inverse" of Opacity: 0=fully opaque, ..., , 255=fully transparent
        /// </summary>
        public byte Transparency => (byte) (255 - Opacity);

        /// <summary>
        /// DXF compliant/compatible transparency value: 0=fully opaque, 90=almost transparent 
        /// values greater 90 will not be accepted (by underlying netdxf).
        /// </summary>
        public short DxfTransparency
        {
            get
            {
                var transparencyPerCent = 100 * Transparency / 255;
                return (transparencyPerCent > 90) ? (short) 90 : (short) transparencyPerCent;
            }
        }

        /// <summary>
        /// Is the symbol visible at all? At this basic level, only transparency/opacity comes
        /// into play. At descendant - more specific - level, properties like line with (0) might
        /// also be of interrest.
        /// </summary>
        public virtual bool IsVisible => (0 < Opacity);
    }
}
