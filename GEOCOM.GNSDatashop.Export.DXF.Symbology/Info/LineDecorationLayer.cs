using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LineDecorationLayer
    {
        protected ISimpleLineDecorationElement _decorationElement;

        protected MarkerSymbolInfo _symbolInfo;

        protected double _dotstoMeter;

        public LineDecorationLayer(ILineDecorationElement decorationElement, MarkerSymbology markerSymbology)
        {
            _decorationElement = decorationElement as ISimpleLineDecorationElement;
            _symbolInfo = markerSymbology.CreateInfo(_decorationElement.MarkerSymbol as ISymbol);
            _dotstoMeter = markerSymbology.DotsToMeter;
        }

        public IEnumerable<LineMarkerPosition> GetPositions(ICurve curve)
        {
            var symbolWidth = _symbolInfo.Block.Extent.Width * _dotstoMeter;

            var positions = new List<LineMarkerPosition>();     // Create temporary list to avoid issues with IPoint (COM-) object lifetime

            if (_decorationElement.PositionCount <= 1)
                positions.Add(GetSinglePosition(curve, symbolWidth));
            else
                positions.AddRange(GetMultiplePositions(curve, symbolWidth));

            return positions;
        }

        private LineMarkerPosition GetSinglePosition(ICurve curve, double symbolWidth)
        {
            double position;
            double length = curve.Length;

            if (0 < _decorationElement.PositionCount)
                position = AbsolutePosition(_decorationElement, 0, length);
            else
                position = length / 2;

            var isAtStartOfLine = (Math.Abs(position) < 1e-06);

            var symbolCenter = symbolWidth / 2;
            if (position < symbolCenter)    // lowest allowed position
                position = symbolCenter;
            else if (position > (length - symbolCenter))
                position = length - symbolCenter;

            return new LineMarkerPosition(curve, position, isAtStartOfLine);
        }

        private IEnumerable<LineMarkerPosition> GetMultiplePositions(ICurve curve, double symbolWidth)
        {
            var lineLength = curve.Length;
            var nPositions = _decorationElement.PositionCount;
            // Assuming the symbol is centered middle/center around the cell origin/coordinare reference point
            for (int i = 0; (i < nPositions); i++)
            {
                var position = AbsolutePosition(_decorationElement, i, lineLength);
                yield return  new LineMarkerPosition(curve, position + SymbolOffset(i, nPositions - 1, symbolWidth), i == 0);
            }
        }

        /// <summary>
        /// Distribute the symbol with among the placements so the start/end symbol are aligned
        /// at the start/end position and the symbols inbetween are equally spaced.
        /// </summary>
        /// <param name="index">Index of given position (0..n)</param>
        /// <param name="nGaps">Number of gaps (number os positions - 1)</param>
        /// <param name="width">Symbol width to distribute</param>
        /// <returns></returns>
        private double SymbolOffset(int index, int nGaps, double width)
        {
            return 0.5 * width - width / ((nGaps > 0) ? nGaps : 1.0) * index;
        }

        public MarkerSymbolInfo MarkerSymbolInfo => _symbolInfo;

        public bool FlipFirst => _decorationElement.FlipFirst;

        public bool FlipAll => _decorationElement.FlipAll;

        public bool Rotate => _decorationElement.Rotate;

        #region private helpers
        private double AbsolutePosition(ISimpleLineDecorationElement element, int index, double fullLength)
        {
            var elementLength = element.Position[index];
            return (element.PositionAsRatio)
                ? elementLength * fullLength
                : elementLength;
        }
        #endregion

    }
}
