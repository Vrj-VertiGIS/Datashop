using System.Collections.Generic;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ITemplateExtensions
    {
        public static IEnumerable<double> LinePattern(this ITemplate lineTemplate)
        {
            if (null != lineTemplate)
                for (int i = 0; (i < lineTemplate.PatternElementCount); i++)
                {
                    double mark; double gap;
                    lineTemplate.GetPatternElement(i, out mark, out gap);
                    if (1E-6 < mark)
                        yield return mark * lineTemplate.Interval;
                    if (1e-6 < gap)
                        yield return (-1) * gap * lineTemplate.Interval;
                }
        }

        public static uint Identity(this ITemplate lineTemplate)
        {
            // Hoping the below is somehow a distinguishing hash func.
            double sumMarks = 0.0;
            for (int i = lineTemplate.PatternElementCount - 1; 0 <= i; i--)
            {
                double mark; double gap;
                lineTemplate.GetPatternElement(i, out mark, out gap);
                sumMarks += (i * mark - (i - 1) * gap); // Prevent adding zero if gap and mark are equal
            }
            return (uint)sumMarks.GetHashCode();
        }
    }
}
