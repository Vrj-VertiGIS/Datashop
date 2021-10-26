using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class ESRIMap : ESRILayers
    {
        public delegate void SelectionChangedEventHandler();
        public delegate void LayersChangedEventHandler();

        public event SelectionChangedEventHandler OnSelectionChanged;
        public event LayersChangedEventHandler OnLayersChanged;

        private IApplication _application = null;

        public ESRIMap(IApplication application)
            : base()
        {
            _application = application;
        }

        public ESRIMap(IApplication application, IMap map)
            : base()
        {
            _application = application;
            Map = map;
        }

        public override IMap Map
        {
            get { return base.Map; }
            set
            {
                UnwireMapEvents();
                base.Map = value;
                WireMapEvents();
            }
        }

        protected override void Reset()
        {
            UnwireMapEvents();
            base.Reset();
        }

        private void UnwireMapEvents()
        {
            if (null != Map)
            {
                try
                {
                    var ve = (IActiveViewEvents_Event)Map;
                    ve.ItemAdded -= OnItemsAlteredEvent;
                    ve.ItemDeleted -= OnItemsAlteredEvent;
                    ve.ItemReordered -= OnItemsReorderedEvent;
                    ve.SelectionChanged -= OnSelectionChangedEvent;
                }
                catch
                {
                    // Ignore any error.
                }
            }
        }

        private void WireMapEvents()
        {
            if (null != Map)
            {
                var ve = (IActiveViewEvents_Event) Map;
                ve.ItemAdded += OnItemsAlteredEvent;
                ve.ItemDeleted += OnItemsAlteredEvent;
                ve.ItemReordered += OnItemsReorderedEvent;
                ve.SelectionChanged += OnSelectionChangedEvent;
            }
        }

        private void OnItemsAlteredEvent(object item)
        {
            ResetLayers();

            OnLayersChanged?.Invoke();
        }

        private void OnItemsReorderedEvent(object item, int index)
        {
            ResetLayers();

            OnLayersChanged?.Invoke();
        }

        private void OnSelectionChangedEvent()
        {
            ResetLayers();

            OnSelectionChanged?.Invoke();
        }

    }
}
