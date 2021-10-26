using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.TTF;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    /// <summary>
    /// Base class implementing symbology - basically wrapping an esri renderer (e.g. FeatureRenderer)
    /// </summary>
    /// <typeparam name="T">Type of symbols (SymbolInfo descendants respectively) dealth with heere</typeparam>
    public abstract class Symbology<T> where T : ISymbolInfo, new()
    {
        protected IFeatureRenderer _esriRenderer = null;
        protected ILookupSymbol _lookupSymbol = null;
        protected ILayer _esriLayer = null;
        protected ITransparencyRenderer _transparencyRenderer = null;

        protected IUniqueValueRenderer _esriUVRenderer = null;

        protected double _dotsToMeter = 0.0;
        public double DotsToMeter => _dotsToMeter;

        protected List<int> _uvFields = new List<int>();    // Indices to lookup value fields
        protected ILayerFields _layerFields = null;

        protected Symbology(ILayer esriLayer, double dotsToMeter)
        {
            _esriLayer = esriLayer;

            _dotsToMeter = dotsToMeter;

            _esriRenderer = (_esriLayer is IGeoFeatureLayer gfl) ? gfl.Renderer : null;
            _transparencyRenderer = (null != _esriRenderer) ? _esriRenderer as ITransparencyRenderer : null;

            _esriUVRenderer = _esriRenderer as IUniqueValueRenderer;

            _layerFields = _esriLayer as ILayerFields;

            if ((null != _esriUVRenderer)
                && (_esriLayer is IFeatureLayer fl)
                && (null != fl.FeatureClass?.Fields))
                GetUVFields(fl.FeatureClass.Fields);
        }

        protected virtual ISymbol LookupSymbol(IFeature feature)
        {
            // when ran as arcmap tool this will be compiled as x86 target
            // and we can utilize the usual symbol lookup
            // shen built and run as engine/arcgis server component, 
            // the below comment applies.

            if ((null != _esriUVRenderer) && Environment.Is64BitProcess)
                // As of Nov. 2017, ILookupSymbol.LookupSymbol() does not work in ArcEngine environments
                // legcy workaround programmed below.
                // By now this has the adverse effect that invisible default symbols will by 
                // drawn anyways - what is an error but cannot be handelt so far.
                return LookupUniqueValueSymbol(feature);
            else
            {
                ISymbol symbol = null;

                if (_lookupSymbol != null)
                    // Use ILookupSymbol instead of IFeatureRenderer.SymbolByFeature
                    // as only this way ISymbol equality will be preserved. 
                    // IFeatureRenderer.SymbolByFeature[] seems to copy the symbol and thus
                    // equality cannot be achieved.
                    symbol = LookupSymbolCore(false, feature);
                else if (null != _esriRenderer)
                {
                    _lookupSymbol = _esriRenderer as ILookupSymbol;
                    if (null != _lookupSymbol)
                        symbol = LookupSymbolCore(true, feature);
                    else
                        // Only if ILookupSymbol is not supported (GEOCOM UV text renderer)
                        symbol = _esriRenderer.SymbolByFeature[feature];
                }

                // Check if to draw the feature in case the default symbol shall _not_ be used
                return (null == _esriUVRenderer) || (_esriUVRenderer.UseDefaultSymbol) || (_esriUVRenderer.DefaultSymbol != symbol)
                    ? symbol
                    : null;
            }
        }

        private ISymbol LookupSymbolCore(bool firstPass, IFeature feature)
        {
            try
            {
                return _lookupSymbol.LookupSymbol(firstPass, feature);
            }
            catch (COMException)
            {
                return null;
            }
        }

        /// <summary>
        /// Lookup symbol for a feature layer rendered by an unique value renderer
        /// </summary>
        /// <param name="feature">Simple feature to lookup symbol for</param>
        /// <returns>ISymbol interface</returns>
        private ISymbol LookupUniqueValueSymbol(IFeature feature)
        {
            var values = new List<string>(_esriUVRenderer.FieldCount);
            for (int i = 0; (i < _esriUVRenderer.FieldCount); i++)
                values.Add(feature.ValueAsString(_uvFields[i], @"<Null>"));
            var symbolValue = string.Join(_esriUVRenderer.FieldDelimiter, values);
            return LookupUniqueValueSymbol(symbolValue);
        }

        /// <summary>
        /// Lookup symbol for value (delimited value string) rendered by an unique value renderer
        /// </summary>
        /// <param name="value">Value (delimited value string)</param>
        /// <returns>ISymbol interface</returns>
        private ISymbol LookupUniqueValueSymbol(string value)
        {
            var symbol = _esriUVRenderer.GroupingAwareSymbol(value);
            return (null != symbol)
                ? symbol
                : _esriUVRenderer.DefaultSymbol;
        }

        /// <summary>
        /// Get the fields required to assign symbols to unique value groups
        /// </summary>
        /// <param name="dataFields"></param>
        private void GetUVFields(IFields dataFields)
        {
            for (int i = 0; (i < _esriUVRenderer.FieldCount); i++)
                _uvFields.Add(_layerFields.FindField(_esriUVRenderer.Field[i]));
        }

        // Important: Call this after processing the symbol retrieved by LookupSymbol(IFeature)
        // as the symbol lookup facility needs to be reset in order to preserve correct symbol
        // angles. 
        protected void ResetLookupSymbol(ISymbol symbol)
        {
            if ((null != _lookupSymbol) && (null != symbol))
            {
                _lookupSymbol.ResetLookupSymbol(symbol);
            }
        }

        /// <summary>
        /// Lookup a symbol corresponding the given feature in the layer's renderer. Return
        /// The symbology info in a SymbolInfo descendant.
        /// </summary>
        /// <param name="feature">Feature for which we want symbology information</param>
        /// <returns>SymbolInfo descendant holding desired info</returns>
        public T CreateInfo(IFeature feature)
        {
            ISymbol symb = null;

            try
            {
                return CreateInfoCore(feature, out symb);
            }
            finally
            {
                ResetLookupSymbol(symb);
            }
        }

        /// <summary>
        /// Create a symbol info when the basic esri symbol is known
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public T CreateInfo(ISymbol symbol)
        {
            return CreateInfoCore(symbol);
        }

        /// <summary>
        /// Create a symbol info when the basic esri symbol is known and the alignment should explicitly be provided 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public T CreateInfo(ISymbol symbol, GlyphAlignment alignment)
        {
            return CreateInfoCore(symbol, alignment);
        }

        /// <summary>
        /// Create a symbolinfo descendant corresponding to the given feature. Lookup the
        /// related symbol (Isymbol) in the renderer. Return the symbol so it can be used 
        /// to reset the renderers internal structures (e.g. rotation)
        /// </summary>
        /// <param name="feature">Feature for which a symbol shall be looked up</param>
        /// <param name="symbol">Looked up symbol (for internal use only)</param>
        /// <returns>SymbolInfo descendant holding desired symbology information</returns>
        protected virtual T CreateInfoCore(IFeature feature, out ISymbol symbol)
        {
            symbol = LookupSymbol(feature);     // Lookup symbol for feature
            return CreateInfoCore(symbol);
        }

        /// <summary>
        /// Implementation in Symbology descendants creating a related SymbolInfo descendant
        /// from passed symbol info.
        /// </summary>
        /// <param name="symbol">Symbol supplying desired info</param>
        /// <returns>SymbolInfo descendant holding symbol info</returns>
        protected abstract T CreateInfoCore(ISymbol symbol);

        /// <summary>
        /// Overwrite this method if you expect CreateInfo(ISymbol symbol, GlyphAlignment alignment) to be called on your objects.
        /// </summary>
        /// <param name="symbol">Symbol supplying desired info</param>
        /// <param name="alignment">Alignment. Currently only implemented by marker symbology</param>
        /// <returns>SymbolInfo descendant holding symbol info</returns>
        protected virtual T CreateInfoCore(ISymbol symbol, GlyphAlignment alignment)
        {
            throw new NotImplementedException("CreateInfoCore(ISymbol symbol, GlyphAlignment alignment) is not overwritten, hence CreateInfo(ISymbol symbol, GlyphAlignment alignment) may not be called.");
        }
    }
}