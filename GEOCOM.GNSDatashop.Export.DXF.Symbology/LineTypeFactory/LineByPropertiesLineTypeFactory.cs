using System.Linq;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.LineTypeFactory
{
    public class LineByPropertiesLineTypeFactory : LinearTypeFactory  
    {
        protected string _name;
        protected ITemplate _lineTemplate;

        public LineByPropertiesLineTypeFactory(string name, ILineSymbol lineSymbol, double dotstoMeter)
            : base(lineSymbol, dotstoMeter)
        {
            _lineTemplate = (lineSymbol as ILineProperties)?.Template;
            _name = name;
        }

        public virtual Linetype ToLine()
        {
            var linetype = NewLineType();

            linetype.Segments.Clear();
            linetype.Segments.AddRange(_lineTemplate.LinePattern().Select(p => (_dotsToMeter * p)));

            return linetype;
        }

        protected virtual Linetype NewLineType()
        {
            return new Linetype(_name);
        }
    }
}
