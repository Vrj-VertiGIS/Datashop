using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class SimpleFillHatchPatternFactory : HatchPatternFactory
    {
        protected ISimpleFillSymbol _simpleFillSymbol;

        public SimpleFillHatchPatternFactory(string name, ISimpleFillSymbol simpleFillSymbol, double dotsToMeter)
            : base(name, simpleFillSymbol as IFillSymbol, dotsToMeter)
        {
            _simpleFillSymbol = simpleFillSymbol;
            _patternLindWidth = base.PatternLineWidth;
        }

        private double _patternLindWidth = 0;

        public override double PatternLineWidth => _patternLindWidth;

        #region protected members

        protected override HatchPattern NewHatchPattern()
        {
            return (_simpleFillSymbol.Style != esriSimpleFillStyle.esriSFSSolid)
                ? new HatchPattern(_name) { Type = HatchType.Custom }   // Cannot clone and then rename patterns, cannot set patterntype (solid, patternfill)
                : HatchPattern.Solid.Clone()  as HatchPattern;
        }

        protected override void AddPatternLines(HatchPattern pattern)
        {
            var fillSymbolStyle = _simpleFillSymbol.Style;
            var patternLines = pattern.LineDefinitions;
            _patternLindWidth = 1.0 * _dotsToMeter;

            switch (fillSymbolStyle)
            {
                case esriSimpleFillStyle.esriSFSBackwardDiagonal:
                    patternLines.AddRange(BackwardDiagonalHatchPattern());
                    break;
                case esriSimpleFillStyle.esriSFSCross:
                    patternLines.AddRange(CrossHatchPattern());
                    break;
                case esriSimpleFillStyle.esriSFSDiagonalCross:
                    patternLines.AddRange(DiagonalCrossHatchPattern());
                    break;
                case esriSimpleFillStyle.esriSFSForwardDiagonal:
                    patternLines.AddRange(ForwardDiagonalHatchPattern());
                    break;
                case esriSimpleFillStyle.esriSFSHorizontal:
                    patternLines.AddRange(RightangularHatchPattern(0, patternLines));
                    break;
                case esriSimpleFillStyle.esriSFSVertical:
                    patternLines.AddRange(RightangularHatchPattern(270, patternLines));
                    break;
            }
        }

        #endregion

        #region private helpers

        private IEnumerable<HatchPatternLineDefinition> BackwardDiagonalHatchPattern()
        {
            return SlantedHatchPattern(135, _dotsToMeter * 6.0);
        }

        private IEnumerable<HatchPatternLineDefinition> ForwardDiagonalHatchPattern()
        {
            return SlantedHatchPattern(45, _dotsToMeter * 6.0);
        }

        private IEnumerable<HatchPatternLineDefinition> SlantedHatchPattern(double dashAngle, double dashLen)
        {
            var hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(0, 0);
            hpd.Angle = dashAngle;
            hpd.Delta = new Vector2(Math.Sqrt(Math.Pow(dashLen, 2) / 2), Math.Sqrt(Math.Pow(dashLen, 2) / 2));
            yield return hpd;
        }

        private IEnumerable<HatchPatternLineDefinition> CrossHatchPattern()
        {
            var dashLen = _dotsToMeter * 12;
            var dashGap = _dotsToMeter * 6.0;
            const double dash1Angle = 0.0;
            const double dash2Angle = 270.0;

            var hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(0, 0);
            hpd.Angle = dash1Angle;
            hpd.Delta = new Vector2(dashLen + dashGap, dashLen + dashGap);
            hpd.DashPattern.Add(dashLen);
            hpd.DashPattern.Add((-1) * dashGap);
            yield return hpd;

            hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(dashLen / 2.0, 0);
            hpd.Angle = dash2Angle;
            hpd.Delta = new Vector2(dashLen + dashGap, dashLen + dashGap);
            hpd.DashPattern.Add(dashLen);
            hpd.DashPattern.Add((-1) * dashGap);
            yield return hpd;
        }

        private IEnumerable<HatchPatternLineDefinition> DiagonalCrossHatchPattern()
        {
            var dashLen = _dotsToMeter * 12;
            var dashGap = _dotsToMeter * 6.0;
            const double dash1Angle = 315.0;
            const double dash2Angle = 45.0;

            var hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(0, 0);
            hpd.Angle = dash1Angle;
            hpd.Delta = new Vector2(Math.Sqrt(Math.Pow(dashLen + dashGap, 2) / 2.0), Math.Sqrt(Math.Pow(dashLen + dashGap, 2) / 2.0));
            hpd.DashPattern.Add(dashLen);
            hpd.DashPattern.Add((-1) * dashGap);
            yield return hpd;

            hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(0, Math.Sqrt(Math.Pow(dashLen / 2.0, 2) / 2.0));
            hpd.Angle = dash2Angle;
            hpd.Delta = new Vector2(Math.Sqrt(Math.Pow(dashLen + dashGap, 2) / 2.0), Math.Sqrt(Math.Pow(dashLen + dashGap, 2) / 2.0));
            hpd.DashPattern.Add(dashLen);
            hpd.DashPattern.Add((-1) * dashGap);
            yield return hpd;
        }

        private IEnumerable<HatchPatternLineDefinition> RightangularHatchPattern(double angle, IList<HatchPatternLineDefinition> patternLines)
        {
            var dashLen = _dotsToMeter * 12;
            var dashOffset = dashLen / 3.0;

            var hpd = new HatchPatternLineDefinition();
            hpd.Origin = new Vector2(0, 0);
            hpd.Angle = angle;
            hpd.Delta = new Vector2(dashLen, dashOffset);
            hpd.DashPattern.Add(dashLen);
            yield return hpd;
        }
        #endregion
    }
}
