using System;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class FillSymbolInfo : SymbolInfo
    {
        protected Color _fillColor = Color.Empty;
        protected HatchPattern _hatchPattern = null;
        protected LayeredLineSymbolInfo _outline = new LayeredLineSymbolInfo();
        protected double _hatchPatternLineWidth = 0.0;

        public FillSymbolInfo(ISymbol symbol, ILayer layer, Tuple<HatchPattern, double> patternInfo, LayeredLineSymbolInfo outline)
            : base(symbol, layer)
        {
            _fillColor = (symbol is IFillSymbol fs)
                ? Color.FromArgb(fs.Color.Transparency, Color.FromArgb(fs.Color.RGB))
                : Color.Empty;

            _hatchPattern = patternInfo.Item1;
            _hatchPatternLineWidth = patternInfo.Item2;

            _outline = outline;
        }

        protected byte FillTransparency => (byte) (255 - FillOpacity);                  // Transparency of this fill [0..1] where 1 is fully transparent
        protected byte FillOpacity => _fillColor.A;                                     // Opacity of this fill [0..255] where 1 is fully opaque 

        public override byte Opacity => (byte) ((LayerOpacity / 255.0) * FillOpacity);      // Overall opacity - including (inherited) layer opacity

        public virtual HatchPattern Pattern => _hatchPattern;

        public virtual double PatternlineWidth => _hatchPatternLineWidth;

        public virtual AciColor DXFColor => _fillColor.AciColor();
		
        public virtual LayeredLineSymbolInfo OutLine => _outline;

        public bool FillIsVisible => base.IsVisible;

        public bool OutlineIsVisible => (_outline.IsVisible);

        public override bool IsVisible => (FillIsVisible || OutlineIsVisible);
    }
}

