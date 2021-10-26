using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Display;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public abstract class HatchPatternLineFactory 
    {
        #region static class methods

        public static HatchPatternLineFactory CreateFactory(ILineSymbol symbol, ILineFillSymbol fillSymbol, double dotsToMeter)
        {
            if (symbol is IMultiLayerLineSymbol mlls)
                return new HatchPatternMultiLayerLineFactory(mlls, fillSymbol, dotsToMeter);
            else if (symbol is ILineProperties props)
                return new HatchPatternLineByPropertiesFactory(props, fillSymbol, dotsToMeter);
            else if (symbol is ISimpleLineSymbol sls)
                return new HatchPatternSimpleLineFactory(sls, fillSymbol, dotsToMeter);
            else if (symbol is IPictureLineSymbol pls)
                return new HatchPatternPictureLineFactory(pls, fillSymbol, dotsToMeter);

            return null;
        }

        #endregion

        protected ILineSymbol _lineSymbol;
        protected ILineFillSymbol _fillSymbol;
        protected double _dotsToMeter;

        public HatchPatternLineFactory(ILineSymbol symbol, ILineFillSymbol fillSymbol, double dotsToMeter)
        {
            _lineSymbol = symbol;
            _fillSymbol = fillSymbol;
            _dotsToMeter = dotsToMeter;
        }

        public abstract IEnumerable<HatchPatternLineDefinition> ToPatternLine();

        #region protected helpers

        /// <summary>
        /// Create a custom hatch pattern line definition
        /// </summary>
        /// <param name="dashDots">An enty length (dots) with positive value is regarted as a dash line
        ///                                              with negative value is regarted as a space</param>
        /// <param name="separationDots">offset between lines</param>
        /// <param name="angle">math. angle of the lines</param>
        /// <param name="patternLines">add freshly created line pattern heere</param>
        /// <returns>HatchPatternLineDefinition</returns>
        protected HatchPatternLineDefinition CustomHatchPatternLine(IEnumerable<double> dashDots, double separationDots, double angle, double linewidth = 1.0)
        {
            var hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(0, 0);
            hpd.Angle = angle;
            hpd.Delta = new Vector2(0, (separationDots) * _dotsToMeter);
            hpd.DashPattern.AddRange(dashDots.Select(d => d * _dotsToMeter * linewidth));
            return hpd;
        }

        /// <summary>
        /// The Offset (2 dimensional vector vector) from a start point of a line to the next one
        /// </summary>
        /// <param name="LineAngleDeg">Perpendicular distance - in dots - to the next line</param>
        /// <param name="linearOffset">Linear distance - in dots - to the next line</param>
        /// <returns></returns>
        protected Vector2 HatchLineOffset(double LineAngleDeg, double linearOffset)
        {
            var rad = LineAngleDeg / 180.0 * Math.PI;
            var dx = Math.Cos(rad);
            var dy = Math.Sin(rad);

            return new Vector2(dy * linearOffset, (-dx) + linearOffset);
        }

        #endregion
    }
}
