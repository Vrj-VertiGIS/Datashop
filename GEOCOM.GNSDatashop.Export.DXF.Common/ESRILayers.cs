using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class ESRILayers
    {

        protected IMap _callerSpecifiedMap = null;
        protected IEnumerable<ILayer> _callerSpecifiedLayers = null;

        private IList<ILayer> _allLayers = null;
        private IList<ILayer> _visibleOnlyLayers = null;

        private double? _mapScale = null;

        public ESRILayers() { }

        public ESRILayers(IMap map)
        {
            _callerSpecifiedMap = map;
            if (Math.Abs(_callerSpecifiedMap.MapScale) > 1E-6)
                _mapScale = Math.Round(_callerSpecifiedMap.MapScale, 3);
        }

        public ESRILayers(IEnumerable<ILayer> layers, double mapScale)
        {
            _callerSpecifiedLayers = layers;
            if (Math.Abs(mapScale) > 1E-6)
                _mapScale = Math.Round(mapScale, 3);
        }

        public virtual IMap Map
        {
            get { return _callerSpecifiedMap; }
            set
            {
                Reset();
                _callerSpecifiedMap = value;
            }
        }

        public IList<ILayer> Layers
        {
            get
            {
                if (null == _allLayers)
                    if (null != _callerSpecifiedMap)
                        PopulateLayerLists(_callerSpecifiedMap);
                    else if (null != _callerSpecifiedLayers)
                        PopulateLayerLists(_callerSpecifiedLayers);

                return _allLayers ?? new List<ILayer>();
            }
        }

        public IList<ILayer> VisibleOnlyLayers
        {
            get
            {
                if (null == _allLayers)
                    if (null != _callerSpecifiedMap)
                        PopulateLayerLists(_callerSpecifiedMap);
                    else if (null != _callerSpecifiedLayers)
                        PopulateLayerLists(_callerSpecifiedLayers);

                return _visibleOnlyLayers ?? new List<ILayer>();
            }
        }

        public IEnumerable<ILayer> LayersByName(string layerName)
        {
            var lyrs = Layers;
            return lyrs.Where(l => l.Name.Equals(layerName, StringComparison.OrdinalIgnoreCase));
        }


        public IEnumerable<ILayer> SelectedLayers => Layers.Where(l => ((l is IFeatureSelection fs) && (null != fs.SelectionSet) && (0 < fs.SelectionSet.Count)));

        protected virtual void Reset()
        {
            ResetLayers();
            _callerSpecifiedMap = null;
        }

        protected virtual void ResetLayers()
        {
            _allLayers = null;
            _visibleOnlyLayers = null;
        }

        private void PopulateLayerLists(IMap map)
        {
            _allLayers = new List<ILayer>();
            _visibleOnlyLayers = new List<ILayer>();

            if (null != map)
                for (int i = 0; i < map.LayerCount; i++)
                {
                    var layer = map.Layer[i];
                    TraverseLayers(layer, layer.Visible);
                }
        }

        private void PopulateLayerLists(IEnumerable<ILayer> layers)
        {
            _allLayers = new List<ILayer>();
            _visibleOnlyLayers = new List<ILayer>();

            foreach (var layer in layers)
                TraverseLayers(layer, layer.Visible);

            _allLayers = _allLayers.Distinct().ToList();
            _visibleOnlyLayers = _visibleOnlyLayers.Distinct().ToList();
        }

        private void TraverseLayers(ILayer lyr, bool parentVisible)
        {
            _allLayers.Add(lyr);

            var thisLayerVisible = parentVisible && LayerIsVisibleByScales(lyr);
            if (thisLayerVisible)
                _visibleOnlyLayers.Add(lyr);

            var cLyr = lyr as ICompositeLayer;
            if (null != cLyr)
                for (int i = 0; (i < cLyr.Count); i++)
                {
                    var cLyrLyr = cLyr.Layer[i];
                    TraverseLayers(cLyrLyr, (thisLayerVisible && cLyrLyr.Visible));
                }
        }

        private bool LayerIsVisibleByScales(ILayer lyr)
        {
            return (!_mapScale.HasValue)    // Map scale given?
                // layer Minimum scale not given or map scale >= layer minimum scale
                // Be aware: scales are quotients (1 over the scale given) -so 
                // layers are drawn when map scale < mimimum scale and map scale > maximum cale
                // e.g: map scale 500 is shown when layer minimum scale is 2500 and maximum scale is 0
                || (((Math.Abs(lyr.MinimumScale) < 1E-6) || (_mapScale.Value <= Math.Round(lyr.MinimumScale,3)))
                // layer maximum scale not given or map scale <= layer maximum scale
                && ((Math.Abs(lyr.MaximumScale) < 1E-6) || (_mapScale.Value >= Math.Round(lyr.MaximumScale,3))));
        }

        #region ArcMap drawing layer - graphicsContainer

        /// <summary>
        /// The ArcMap drawing layer elements
        /// </summary>
        public IEnumerable<IElement> DrawingElements
            => (Map is IGraphicsContainer container)
                ? Elements(container)
                : new List<IElement>();

        private IEnumerable<IElement> Elements(IGraphicsContainer container)
        {
            foreach (var element in ContainedElements(container))
                if (element is IGroupElement groupElement)
                    foreach (var containedElement in GroupedElements(groupElement))
                        yield return containedElement;
                else
                    yield return element;
        }

        private IEnumerable<IElement> ContainedElements(IGraphicsContainer container)
        {
            container.Reset();

            for (IElement element; null != (element = container.Next());)
                yield return element;
        }

        private IEnumerable<IElement> GroupedElements(IGroupElement groupElement)
        {
            for (IElement element; null != (element = groupElement.Elements.Next());)
                yield return element;
        }
        #endregion

    }
}
