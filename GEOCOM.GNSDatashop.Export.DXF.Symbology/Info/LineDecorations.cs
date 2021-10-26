using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LineDecorations
    {
        protected ILineProperties _lineProperties;

        protected MarkerSymbology _markerSymbology;

        protected IEnumerable<LineDecorationLayer> _layers = null;

        public LineDecorations(ILineProperties lineProperties, MarkerSymbology markerSymbology)
        {
            _lineProperties = lineProperties;
            _markerSymbology = markerSymbology;
        }

        public LineDecorations()
        {
            _lineProperties = null;
            _markerSymbology = null;
        }


        public bool Ontop => (null != _lineProperties) ? _lineProperties.DecorationOnTop : false;

        public IEnumerable<LineDecorationLayer> DecorationLayers
        {
            get
            {
                // Make shure we don't query the whole thing multiple times
                if (null == _layers)
                    _layers = CreateLayers();

                return _layers;
            }
        }

        private IEnumerable<LineDecorationLayer> CreateLayers()
        {
            return DecorationElements.Where(elm => elm.PositionCount > 0).Select(elm => new LineDecorationLayer(elm, _markerSymbology));
        }

        private IEnumerable<ILineDecorationElement> DecorationElements
        {
            get
            {
                var decorationLayers = _lineProperties?.LineDecoration;
                var elementCount = (null != decorationLayers) ? decorationLayers.ElementCount : 0;
                for (int i = 0; i < elementCount; i++)
                    yield return decorationLayers.Element[i];
            }
        }
    }
}
