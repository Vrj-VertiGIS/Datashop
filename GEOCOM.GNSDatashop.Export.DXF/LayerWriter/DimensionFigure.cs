using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.Factories;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;    
using netDxf.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    public class DimensionFigure
    {
        // Source memvers from layer writer
        private IDimensionShape _shape;
        private DimensionSymbolInfo _symbolInfo;
        private IDimensionGraphic _dimensionGraphic;
        private DxfDimensionLayerWriter _layerWriter;
        private IDimensionFeature _dimensionFeature;
        private ExpressionParsers _expressionParsers;

        // Derived helper members
        private IPoint _dimensionLineBeginPoint;
        private IPoint _dimensionLineEndPoint;
        private IPoint _dimensionLineVector;
        private IPoint _textPoint;
        private double _textAngle;
        private double _textLength;
        private double _avgCharLength;
        private double _dimensionLength;
        private bool _outerText;
        private bool _outerDimensionLines;
        private bool _writeText;

        // Result buffer
        private List<EntityObject> _entities = null;

        #region construction
        public DimensionFigure(IDimensionFeature dimensionFeature, IDimensionGraphic dimensionGraphic, DimensionSymbolInfo symbolInfo, DxfDimensionLayerWriter layerWriter, ExpressionParsers expressionParsers)
        {
            _dimensionFeature = dimensionFeature;
            _shape = dimensionFeature.DimensionShape;
            _symbolInfo = symbolInfo;
            _dimensionGraphic = dimensionGraphic;
            _layerWriter = layerWriter;
            _expressionParsers = expressionParsers;
        }

        #endregion

        #region public properties 

        public IEnumerable<EntityObject> DxfEntities => (null == _entities)
            ? _entities = ToDxfEntities()
            : _entities;

        #endregion

        #region dfx entities from arc objects data

        private List<EntityObject> ToDxfEntities()
        {
            _entities = new List<EntityObject>();

            Prepare();
            DimensionLines();
            ExtensionLines();
            DimensionMarkers();
            DimensionText();

            return _entities;
        }

        private void DimensionLines()
        {
            if (null != _symbolInfo.BeginDimensionLine)
                DimensionLine(_dimensionLineBeginPoint, _dimensionLineVector.Mirror(),
                    (_symbolInfo.TextFit == esriDimensionTextFit.esriDimensionTextFitMoveBegin), _symbolInfo.BeginDimensionLine, _symbolInfo.BeginMarker);
            if (null != _symbolInfo.EndDimensionLine)
                DimensionLine(_dimensionLineEndPoint, _dimensionLineVector,
                    (_symbolInfo.TextFit == esriDimensionTextFit.esriDimensionTextFitMoveEnd), _symbolInfo.EndDimensionLine, _symbolInfo.EndMarker);
        }

        private void DimensionLine(IPoint toPoint, IPoint vector, bool textSide, LayeredLineSymbolInfo symbolInfo, MarkerSymbolInfo markerSymbol)
        {
            if (_outerDimensionLines)
                DimensionLineOuter(toPoint, vector.Mirror(), textSide, symbolInfo, markerSymbol);
            else
                DimensionLineInner(toPoint, vector, symbolInfo);
        }

        private void DimensionLineOuter(IPoint toPoint, IPoint vector, bool textSide, LayeredLineSymbolInfo symbolInfo, MarkerSymbolInfo markerSymbol)
        {
            IPoint tp; 
            var lineLength = (_outerText && textSide && (!(tp = _textPoint.ProjectPointOnLine(toPoint, toPoint.Add(vector))).IsNullOrEmpty()))
                ? tp.Subtract(toPoint).Magnitude() + 0.5 * _textLength
                : 2 * MarkerSize(markerSymbol);
            DimensionLine(toPoint, vector, lineLength, symbolInfo);
        }

        private void DimensionLineInner(IPoint toPoint, IPoint vector, LayeredLineSymbolInfo symbolInfo)
        {
            DimensionLine(toPoint, vector, _dimensionLength * 0.5, symbolInfo);
        }

        private void ExtensionLines()
        {
            if (null != _symbolInfo.BeginExtensionLine)
                ExtensionLine(_shape.BeginDimensionPoint, _dimensionLineBeginPoint, _symbolInfo.ExtensionLineOvershot, _symbolInfo.BeginExtensionLine);
            if (null != _symbolInfo.EndExtensionLine)
                ExtensionLine(_shape.EndDimensionPoint, _dimensionLineEndPoint, _symbolInfo.ExtensionLineOvershot, _symbolInfo.EndExtensionLine);
        }

        private void DimensionMarkers()
        {
            if (null != _symbolInfo.BeginMarker)
                DimensionMarker(_dimensionLineBeginPoint, _dimensionLineVector.Mirror(), _symbolInfo.BeginMarker);

            if (null != _symbolInfo.EndMarker)
                DimensionMarker(_dimensionLineEndPoint, _dimensionLineVector, _symbolInfo.EndMarker);
        }

        private void DimensionText()
        {
            if (_writeText)
                DimensionText(_textPoint, _textAngle, _symbolInfo.TextSymbol, EscapedDimension);
        }

        #endregion

        #region exf entity construction helpers

        private void ExtensionLine(IPoint footPoint, IPoint topPoint, double overshoot, LayeredLineSymbolInfo symbolInfo)
        {
            if (Math.Abs(overshoot) > 1E-6)
                topPoint = topPoint.Add(topPoint.Subtract(footPoint).Offset(overshoot));

            DimLine(footPoint, topPoint, symbolInfo);
        }

        private void DimensionLine(IPoint toPoint, IPoint vector, double length, LayeredLineSymbolInfo symbolInfo)
        {
            var vec = vector.Offset(length);
            DimLine(toPoint.Subtract(vec), toPoint, symbolInfo);
        }

        private void DimLine(IPoint begin, IPoint end, LayeredLineSymbolInfo symbolInfo)
        {
            var line = new PolylineClass() { FromPoint = begin, ToPoint = end };
            _entities.AddRange(EntityFactory.CreatePolyline(line, symbolInfo));
        }

        private void DimensionText(IPoint textPoint, double textAngle, TextSymbolInfo symbolInfo, string dimensionTextString)
        {
            symbolInfo.ReferencePoint = textPoint.ToVector2();
            symbolInfo.Set_Angle(textAngle);
            _entities.Add(EntityFactory.CreateMText(symbolInfo, dimensionTextString));
        }

        private void DimensionMarker(IPoint point, IPoint vector, MarkerSymbolInfo symbolInfo)
        {
            var rot = (_outerDimensionLines)
                ? 180 / Math.PI * Math.Atan2((-1) * vector.Y, (-1) * vector.X)
                : 180 / Math.PI * Math.Atan2(vector.Y, vector.X);

            _entities.Add(EntityFactory.CreateBlockInsert(symbolInfo.Block.Block, rot, point, symbolInfo));
        }

        #endregion

        #region instance helpers

        private double DotsToMeter => _layerWriter.DotsToMeter;

        private DxfEntityFactory EntityFactory => _layerWriter.EntityFactory;

        private double DimensionLength => _dimensionLineVector.Magnitude();

        private double SpaceBetweenMarkers => DimensionLength
            - ((null != _symbolInfo.BeginMarker) ? _symbolInfo.BeginMarker.SizeInDots * DotsToMeter : 0)
            - ((null != _symbolInfo.EndMarker) ? _symbolInfo.EndMarker.SizeInDots * DotsToMeter : 0);

        private IPoint GetTextPoint()
        {
            if (!_shape.TextPoint.IsNullOrEmpty())
                return _shape.TextPoint;
            else
            {
                var textPoint = _dimensionGraphic.GetDefaultTextPoint();
                if (textPoint.IsNullOrEmpty())
                    textPoint = _dimensionLineBeginPoint.Add(_dimensionLineEndPoint).Multiply(0.5);

                return (textPoint.HasNullXYCoordinates())      // a TextPoint of (0,0) signals not to write a text at all
                    ? textPoint
                    : (_outerText)
                        ? TextPointOnOuterside(textPoint)
                        : (_outerDimensionLines)
                            ? _dimensionLineBeginPoint.Add(_dimensionLineVector.Offset(0.5 * _dimensionLineVector.Magnitude()))
                            : textPoint;
            }
        }

        private IPoint TextPointOnOuterside(IPoint textPoint) 
            => (_symbolInfo.TextFit == esriDimensionTextFit.esriDimensionTextFitMoveBegin)
                ? _dimensionLineBeginPoint.Subtract(_dimensionLineVector.Offset(_avgCharLength + _textLength)).Subtract(VectorToDimensionLine(textPoint))
                : (_symbolInfo.TextFit == esriDimensionTextFit.esriDimensionTextFitMoveEnd)
                    ? _dimensionLineEndPoint.Add(_dimensionLineVector.Offset(_avgCharLength + _textLength)).Subtract(VectorToDimensionLine(textPoint))
                    : textPoint;

        private IPoint VectorToDimensionLine(IPoint pt)
        {
            var ppt = pt.ProjectPointOnLine(_dimensionLineBeginPoint, _dimensionLineEndPoint);
            return (!ppt.IsNullOrEmpty())
                ? ppt.Subtract(pt)
                : new ESRI.ArcGIS.Geometry.Point() { X = 0.0, Y = 0.0 };
        }

        private double TextLength(string text, Font font)
            => _layerWriter.Graphics.MeasureString(text, font, int.MaxValue, StringFormat.GenericTypographic).Width * DotsToMeter;

        private double MarkerSize(MarkerSymbolInfo symbolInfo) => (symbolInfo?.SizeInDots ?? 10) * DotsToMeter;

        private void Prepare()
        {
            // Prepare a clear, straight situation
            _dimensionLineBeginPoint = (!_shape.DimensionLinePoint.IsNullOrEmpty())
                ? _shape.DimensionLinePoint
                : _shape.BeginDimensionPoint;

            _dimensionLineEndPoint = (_shape.BeginDimensionPoint.IsNullOrEmpty() || (0 == _shape.BeginDimensionPoint.Compare(_shape.DimensionLinePoint)))
                ? _shape.EndDimensionPoint
                : GetEndDimensionLinePoint(_shape.BeginDimensionPoint, _shape.EndDimensionPoint, _shape.DimensionLinePoint, _shape.ExtensionLineAngle);

            _dimensionLineVector = _dimensionLineEndPoint.Subtract(_dimensionLineBeginPoint);

            _textAngle = GetTextAngle(_shape.TextAngle, _dimensionLineVector, _symbolInfo);
            _textLength = TextLength(FormattedDimension, _symbolInfo.TextSymbol.Font);
            _avgCharLength = 0.5 * TextLength("a", _symbolInfo.TextSymbol.Font);  // Empirically ;-)
            _dimensionLength = DimensionLength;

            _outerDimensionLines = (_symbolInfo.MarkerFit != esriDimensionMarkerFit.esriDimensionMarkerFitNone) && (_textLength > SpaceBetweenMarkers);
            _outerText = _textLength > _dimensionLength;

            _textPoint = GetTextPoint();
            _writeText = !_textPoint.HasNullXYCoordinates();
            _outerDimensionLines = _outerDimensionLines && _writeText;      // Wether to write text has an influence on where the dim.-lines will be drawn
        }

        #endregion

        #region private static helpers

        private static double GetTextAngle(double shapeTextAngle, IPoint dimLineVector, DimensionSymbolInfo symbolInfo)
            => (symbolInfo.AlignToDimensionLine)
                ? MathSnippets.NormalizeDeg(180 / Math.PI * Math.Atan2(dimLineVector.Y, dimLineVector.X))
                : MathSnippets.NormalizeDeg(180 / Math.PI * shapeTextAngle); // Or do we have to use symbolInfo.Text.Angle instead?

        private static IPoint GetEndDimensionLinePoint(IPoint beginDimensionPoint, IPoint endDimensionPoint, IPoint beginDimensionLinePoint, double dimensionLineAngle)
        {
            var extensionLineVector = beginDimensionLinePoint.Subtract(beginDimensionPoint);

            var beginEextensionLine = NewLineByPointAndVector(beginDimensionPoint, extensionLineVector);    // Extension line (dummy) at begin of dimensioning line
            var endExtensionLine = NewLineByPointAndVector(endDimensionPoint, extensionLineVector);         // Extension line (dummy) at end of dimensioning line

            beginEextensionLine.ReverseOrientation();                
            var dimensionLineMidPointConstruct = new ESRI.ArcGIS.Geometry.Point() as IConstructPoint;
            dimensionLineMidPointConstruct.ConstructDeflection(beginEextensionLine, 100, dimensionLineAngle);

            var dimensionLine = new ESRI.ArcGIS.Geometry.Line() { FromPoint = beginDimensionLinePoint, ToPoint = dimensionLineMidPointConstruct as IPoint };

            return dimensionLine.SimpleLineLineIntersection(endExtensionLine);
        }

        private static ILine NewLineByPointAndVector(IPoint fromPoint, IPoint vector) =>
            new ESRI.ArcGIS.Geometry.Line() { FromPoint = fromPoint, ToPoint = fromPoint.Add(vector) };

        #endregion

        #region dimension text formatting

        private string _formattedDimension = null;
        /// <summary>
        /// Measure formatted according to dimension style and feature settings - prefix, digits, suffix
        /// </summary>
        private string FormattedDimension => (null == _formattedDimension)
            ? _formattedDimension = (_dimensionFeature.UseCustomLength)
                ? FormattedMeasure(_dimensionFeature.CustomLength, _symbolInfo)
                : _symbolInfo.TextSymbol.Text
            : _formattedDimension;

        private string _escapedDimension = null;
        /// <summary>
        /// Measure formatted (FormattedDimension) _and_ escaped (dxf font escaping) 
        /// </summary>
        private string EscapedDimension => (null == _escapedDimension)
            ? _escapedDimension = (_dimensionFeature.UseCustomLength)
                ? EscapedMeasure(_dimensionFeature.CustomLength, _symbolInfo)
                : string.Join(string.Empty, MTextLabelTokenWriter.GetFormattedText(_symbolInfo.TextSymbol.TextTokens))
            : _escapedDimension;

        private string EscapedMeasure(double measure, DimensionSymbolInfo symbolInfo)
        {
            var fmtTxt = FormattedMeasure(measure, symbolInfo);
            return UpdatedLabelText(symbolInfo.TextSymbol, fmtTxt);
        }

        private string FormattedMeasure(double measure, DimensionSymbolInfo symbolInfo)
        {
            switch (symbolInfo.TextDisplay)
            {
                case esriDimensionTextDisplay.esriDimensionTDPrefixSuffix:
                    return $"{symbolInfo.TextPrefix}{FormattedMeasure(measure, symbolInfo.DisplayPrecision)}{symbolInfo.TextSuffix}";
                case esriDimensionTextDisplay.esriDimensionTDValueOnly:
                    return FormattedMeasure(measure, symbolInfo.DisplayPrecision);
                case esriDimensionTextDisplay.esriDimensionTDExpression:
                    return EvaluatedExpression(symbolInfo.ParserName, symbolInfo.TextExpression, symbolInfo.ExpressionSimple, _dimensionFeature as IFeature);
                case esriDimensionTextDisplay.esriDimensionTDNone:
                    return string.Empty;
                default:
                    return FormattedMeasure(measure, symbolInfo.DisplayPrecision);
            }
        }

        private static string UpdatedLabelText(TextSymbolInfo symbolInfo, string text)
        {
            symbolInfo.Set_Text(text);
            return string.Join(string.Empty, MTextLabelTokenWriter.GetFormattedText(symbolInfo.TextTokens));
        }

        private static string FormattedMeasure(double measure, int displayPrecision)
            => string.Format($"{{0{MeasureFormat(displayPrecision)}}}", measure); 

        private static string MeasureFormat(int displayPrecision)
            => $":F{displayPrecision}";

        private string EvaluatedExpression(string parserName, string expression, bool simple, IFeature feature)
        {
            string label = string.Empty;

            var pEngine = _expressionParsers[parserName];

            if (null != pEngine)
            {
                var parser = (simple)
                    ? pEngine.SetExpression(string.Empty, expression)
                    : pEngine.SetCode(expression, FuncName(expression));

                var pEx = parser.Expression;

                label = (null != parser)
                    ? EvaluateExpression(parser, pEx, feature)
                    : string.Empty;
            }
            return label;
        }

        private string FuncName(string expr)
        {
            var tokens = ExprTokens(expr).GetEnumerator();
            return (tokens.MoveNext() && tokens.Current.Equals("Function", StringComparison.OrdinalIgnoreCase)
                && tokens.MoveNext() && !string.IsNullOrEmpty(tokens.Current))
                ? tokens.Current
                : string.Empty;
        }

        private IEnumerable<string> ExprTokens(string expr)
            => expr.Trim().Split(new char[] { ' ', '(' }, StringSplitOptions.RemoveEmptyEntries);

        private string EvaluateExpression(IAnnotationExpressionParser parser, string expression, IFeature feature)
        {
            try
            {
                var label = parser.FindLabel(feature);
                return label;
            }   
            catch (Exception ex)
            {
                int errorNumber = 0;
                int line = 0;
                string description = string.Empty;
                parser.LastError(ref errorNumber, ref line, ref description);
                throw new Exception($"Expression parser error {errorNumber} @ line {line}: {description}", ex);
            }
        }

        private static object FieldValue(IFeature feature, string name)
            => feature.Value[feature.Fields.FindField(name)];

        #endregion

    }
}
