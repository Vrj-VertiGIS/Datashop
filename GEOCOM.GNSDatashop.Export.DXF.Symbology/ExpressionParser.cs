using ESRI.ArcGIS.Carto;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class ExpressionParsers
    {
        #region Expression parsers for annotations and dimensionings

        public IAnnotationExpressionEngine this[string name]
        {
            get => (IsVBScriptParser(name))
                ? VBScriptEngine
                : (IsJScriptParser(name))
                    ? JScriptEngine
                    : (IsPythonParser(name))
                        ? PythonEngine
                        : throw new Exception($"Parser {name} not supported");
            set => Set_Parser(value);
        }

        public bool Available(string name)
            => (IsVBScriptParser(name))
                ? null != _vbscriptEngine
                : (IsJScriptParser(name))
                    ? null != _jscriptEngine
                    : (IsPythonParser(name))
                        ? null != _pythonEngine
                        : false;

        public void Set_Parser(IAnnotationExpressionEngine engine)
        {
            var name = engine.Name;
            if (IsVBScriptParser(name))
                _vbscriptEngine = engine;
            else if (IsJScriptParser(name))
                _jscriptEngine = engine;
            else if (IsPythonParser(name))
                _pythonEngine = engine;
            else
                throw new Exception($"Unknown expression engine: {name}");
        }

        private IAnnotationExpressionEngine _vbscriptEngine = null;
        private IAnnotationExpressionEngine VBScriptEngine
            => (null == _vbscriptEngine)
                ? _vbscriptEngine = new AnnotationVBScriptEngineClass() as IAnnotationExpressionEngine
                : _vbscriptEngine;
        private IAnnotationExpressionEngine _jscriptEngine = null;
        private IAnnotationExpressionEngine JScriptEngine
            => (null == _jscriptEngine)
                ? _jscriptEngine = new AnnotationJScriptEngineClass() as IAnnotationExpressionEngine
                : _jscriptEngine;
        private IAnnotationExpressionEngine _pythonEngine = null;
        private IAnnotationExpressionEngine PythonEngine
            => (null == _pythonEngine)
                ? _pythonEngine = new AnnotationPythonEngineClass() as IAnnotationExpressionEngine
                : _pythonEngine;
        #endregion

        #region static helpers

        private static bool IsVBScriptParser(string name)
            => name.Equals("VB Script", StringComparison.OrdinalIgnoreCase)
            || name.Equals("VBScript", StringComparison.OrdinalIgnoreCase);
        private static bool IsJScriptParser(string name)
            => name.Equals("J Script", StringComparison.OrdinalIgnoreCase)
            || name.Equals("JScript", StringComparison.OrdinalIgnoreCase);
        private static bool IsPythonParser(string name)
            => name.Equals("Python", StringComparison.OrdinalIgnoreCase);

        #endregion
    }
}
