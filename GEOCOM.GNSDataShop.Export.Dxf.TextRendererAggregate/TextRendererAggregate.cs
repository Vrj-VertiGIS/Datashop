using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace GEOCOM.GNSDatashop.Export.DXF.TextRendererAggregate
{
    public class TextRendererAggregate : IUVTRenderer, IUVTRenderer1
    {
        private dynamic _uvtTextRenderer = null;
        private dynamic _uvtTextRenderer1 = null;

        public TextRendererAggregate(IFeatureRenderer renderer)
        {
            _uvtTextRenderer = Qi(renderer, typeof(IUVTRenderer).GUID);
            _uvtTextRenderer1 = Qi(renderer, typeof(IUVTRenderer1).GUID);
        }

        string IUVTRenderer.TextField
        {
            get { return _uvtTextRenderer.TextField; }
            set { _uvtTextRenderer.TextField = value; }
        }

        string IUVTRenderer.ValueField
        {
            get { return _uvtTextRenderer.ValueField; }
            set { _uvtTextRenderer.ValueField = value; }
        }

        string IUVTRenderer.ValueField2
        {
            get { return _uvtTextRenderer.ValueField2; }
            set { _uvtTextRenderer.ValueField2 = value; }
        }

        string IUVTRenderer.AlignField
        {
            get { return _uvtTextRenderer.AlignField; }
            set { _uvtTextRenderer.AlignField = value; }
        }

        long IUVTRenderer.ValueCount
        { get { return _uvtTextRenderer.ValueCount; } }

        int IUVTRenderer.TransparentLayerCorrection
        {
            get { return _uvtTextRenderer.TransparentLayerCorrection; }
            set { _uvtTextRenderer.TransparentLayerCorrection = value; }
        }
        ISymbol IUVTRenderer1.AnchorPointSymbol
        {
            get { return _uvtTextRenderer1.AnchorPointSymbol; }
            set { _uvtTextRenderer.AnchorPointSymbol = value; }
        }
        bool IUVTRenderer1.ShowAnchorPoint
        {
            get { return _uvtTextRenderer1.ShowAnchorPoint; }
            set { _uvtTextRenderer.ShowAnchorPoint = value; }
        }

        void IUVTRenderer.addValue(string value, string heading, ITextSymbol textSymbol)
        {
            _uvtTextRenderer.addValue(value, heading, textSymbol);
        }

        bool IUVTRenderer.GetDefaultTextSymbol(ref ITextSymbol textSymbol, ref string heading)
        {
            return _uvtTextRenderer.GetDefaultTextSymbol(ref textSymbol, ref heading);
        }

        string IUVTRenderer.GetHeading(long index)
        {
            return _uvtTextRenderer.GetHeading(index);
        }

        bool IUVTRenderer1.getPointerLineSymb(ref ISymbol defaultLineSymbol)
        {
            return _uvtTextRenderer1.getPointerLineSymb(defaultLineSymbol);
        }

        bool IUVTRenderer1.getPointerLineSymbByVal(string value, ref ISymbol pointerLineSymbol)
        {
            return _uvtTextRenderer1.getPointerLineSymbByVal(value, ref pointerLineSymbol);
        }

        void IUVTRenderer1.getSymbolsByFeature(IFeature textFeature, ref ISymbol fixTextSymbol, ref ISymbol pointerLineSymbol)
        {
            _uvtTextRenderer1.getSymbolsByFeature(textFeature, ref fixTextSymbol, ref pointerLineSymbol);
        }

        ITextSymbol IUVTRenderer.GetTextSymbol(string value)
        {
            return _uvtTextRenderer.GetTextSymbol(value);
        }

        ITextSymbol IUVTRenderer.GetTextSymbolByIndex(long index)
        {
            return _uvtTextRenderer.GetTextSymbolByIndex(index);
        }

        string IUVTRenderer.GetValue(long index)
        {
            return _uvtTextRenderer.GetValue(index);
        }

        void IUVTRenderer.InitTextRenderer()
        {
            _uvtTextRenderer.InitTextRenderer();
        }

        private object Qi(object from, Guid riid)
        {
            var pUnkFrom = Marshal.GetIUnknownForObject(from);
            IntPtr ppv = IntPtr.Zero;
            var hr = Marshal.QueryInterface(pUnkFrom, ref riid, out ppv);
            if (hr != 0)
                return null;

            return Marshal.GetObjectForIUnknown(ppv);
        }

        void IUVTRenderer.SetDefaultTextSymbol(ITextSymbol textSymbol, string heading, bool hasDefaultSymbol)
        {
            _uvtTextRenderer.SetDefaultTextSymbol(textSymbol, heading, hasDefaultSymbol);
        }

        bool IUVTRenderer1.setPointerLineSymb(bool showPointerLine, ISymbol defaultLineSymbol)
        {
            return _uvtTextRenderer1.setPointerLineSymb(showPointerLine, defaultLineSymbol);
        }

        bool IUVTRenderer1.setPointerLineSymbByVal(string value, ISymbol pointerLineSymbol)
        {
            return _uvtTextRenderer1.setPointerLineSymbByVal(value, pointerLineSymbol);
        }
    }
}