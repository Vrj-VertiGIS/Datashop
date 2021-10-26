using GEOCOM.GNSDatashop.Export.DXF.Common.Circular;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public abstract class LineTemplate : CircularList<double>
    {
        protected LineTemplate(IEnumerable<double> marksAndGaps)
            : base(marksAndGaps) { }

        public IEnumerable<Mark> GetMarks(double curveLength, double dotsToMeter)
        {
            var markPosition = 0.0;
            var marksAndGaps = GetEnumerator();
            while ((markPosition < curveLength) && marksAndGaps.MoveNext())
            {
                var markOrGap = marksAndGaps.Current * dotsToMeter;
                if (0 < markOrGap)  // Marks are positive values, gaps negative
                    yield return SetMark(markPosition, markOrGap, curveLength);
                else
                    markOrGap *= (-1);  // make positive
                markPosition += markOrGap;
            }
        }

        private Mark SetMark(double positionInCurve, double markLength, double curveLength)
        {
            var maxMarkLength = curveLength - positionInCurve;
            var trueMarkLength = (maxMarkLength > markLength) ? markLength : maxMarkLength;
            return new Mark() { Position = positionInCurve, Length = trueMarkLength };
        }
    }

    public class Mark
    {
        public double Position { get; internal set; }
        public double Length { get; internal set; }
    }

}
