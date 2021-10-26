using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.Factories;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using netDxf.Tables;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    public abstract class DxfLayerWriterBase : DxfDataWriterBase
    {
        private DxfEntityFactory _dxfEntityFactory = null;     // Do not exposte to interitors!!

        protected ESRIFeatureList _features = null;

        protected int _featuresWritten = 0;
        public int FeaturesWritten
            => _featuresWritten;

        protected Layer _dxfLyr = null;
        protected IFeatureLayer _esriLyr = null;

        private DxfBlockFactory _blockFactory = null;

        internal DxfLayerWriterBase(_IDxfWriterContext context, IFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context)
        {
            _esriLyr = esriLyr;
            _dxfLyr = dxfLayer;
            _features = features;
            base.Init();
        }

        public void WriteLayer()
        {
            PrepareLayer();

            foreach (var feature in _features.Features)
                if (Continue)
                    WriteFeature(feature);
                else
                    break;
        }

        /// <summary>
        /// Do the real job of exporting a single feature to dxf heere
        /// </summary>
        /// <param name="feature"></param>
        internal virtual void WriteFeature(IFeature feature)
        {
            _featuresWritten++;
        }

        /// <summary>
        /// Conversion factor. Transform measures (font heights, line widths,...) given in dots (1/72")
        /// to metric values such that when printed/plotted at the given scale the measures will be 
        /// in the original dot size.
        /// </summary>
        public override double DotsToMeter => 
            (null == _esriLyr) || _esriLyr.ScaleSymbols
                ? DotsToMeterScaled
                : DotsToMeterUnscaled;

        /// <summary>
        /// Features are 2D or 3D?
        /// </summary>
        public bool Is3DData 
        {
            get
            {
                if (_features == null)
                    return false;
                else
                    return _features.CoordinateDimension.Equals(ESRIFeatureList.CoordinateDim.ThreeD);
            }
        }

        /// <summary>
        /// Create dxf entity factory - on demand. In some occasions the required scaling factor (dotsToMeter)
        /// will not be computable upon creation of the writer object and will be available later.
        /// </summary>
        public DxfEntityFactory EntityFactory => _dxfEntityFactory
            ?? (_dxfEntityFactory = Is3DData
                ? new Dxf3DEntityFactory(_dxfLyr, DotsToMeter) as DxfEntityFactory
                : new Dxf2DEntityFaxtory(_dxfLyr, DotsToMeter) as DxfEntityFactory);

        /// <summary>
        /// Create dxf block factory - on demand.
        /// </summary>
        public DxfBlockFactory BlockFactory
        {
            get
            {
                if (null == _blockFactory)
                    _blockFactory = new DxfBlockFactory(_context.RenderDisplay.DisplayTransformation, new IntPtr(_context.RenderDisplay.hDC), _dxfLyr);

                return _blockFactory;
            }
        }

        #region prepare invisible layer for drawing

        /// <summary>
        /// In certain cases, where an invisible layer shall be exported ("export visible layers only" option deactivated) we
        /// need to prepare the invisible layer such that the respective layer will find the correct symbols (i.e. uvt renderer).
        /// </summary>
        private void PrepareLayer()
        {
            if ((_context.RunMode == DxfWriterRunMode.Batch))//  || (!_context.VisibleOnlyLayers.Any(l => l == _esriLyr)))
                _esriLyr.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, _context.RenderDisplay, _context.CancelTracker.NativeObject);
        }

        #endregion

    }
}
