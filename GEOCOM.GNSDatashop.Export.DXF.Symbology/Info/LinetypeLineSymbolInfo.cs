using System.Drawing;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using ESRI.ArcGIS.Display;
using netDxf;
using netDxf.Tables;
using ESRI.ArcGIS.Carto;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LinetypeLineSymbolInfo : LineSymbolInfo
    {
        // Used to have a "default" linesymbology to create raw dxf lines 
        // i.e. lines where symbology does not matter
        private static LinetypeLineSymbolInfo _default = null;  

        protected Color _outLineRGBColor = Color.Empty;
        protected Linetype _lineType = Linetype.ByLayer;
        protected double _widthInDots = 0.0;

        public LinetypeLineSymbolInfo()   
            : base()
        {
        }

        public LinetypeLineSymbolInfo(ISymbol symbol, ILayer layer, Linetype linetype, LineDecorations lineDecoration)
            : base(symbol, layer, lineDecoration)
        {
            _lineType = linetype;
            _lineDecoration = lineDecoration;

            if (symbol is ILineSymbol ls)
            {
                var color = ls.Color;
                _outLineRGBColor = Color.FromArgb(ls.Color.Transparency, Color.FromArgb(ls.Color.RGB));
                _widthInDots = ls.Width;
            }
        }

        protected override byte LineOpacity  => _outLineRGBColor.A;                           // Opacity of this line [0..255] where 255 is fully opaque 

        public virtual double WidthInDots => _widthInDots;

        public virtual Linetype LineType => _lineType;

        public virtual AciColor DXFColor => _outLineRGBColor.AciColor();

        public override bool IsVisible => base.IsVisible
            && (null != _lineType)
            && (0 < _widthInDots)
            && (!IsNullLine);

        public static LinetypeLineSymbolInfo Default => (null == _default) ? _default = new LinetypeLineSymbolInfo() : _default;

        private bool IsNullLine => (_symbol is ISimpleLineSymbol sls) && (sls.Style == esriSimpleLineStyle.esriSLSNull);

    }
}