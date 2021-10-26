using ESRI.ArcGIS.Carto;
using System;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls
{
    public class LayerSelectionComboItem : IEquatable<LayerSelectionComboItem>, IEquatable<ILayer>
    {
        private ILayer _layer;

        public LayerSelectionComboItem(ILayer layer)
        {
            _layer = layer;
        }

        public ILayer Layer => _layer;

        public esriGeometryType GeometryType => ((null != _layer) && (_layer is IFeatureLayer fl))
            ? fl.FeatureClass.ShapeType
            : esriGeometryType.esriGeometryNull;

        public bool Equals(LayerSelectionComboItem other)
        {
            return (null != other)
                ? Equals(_layer, other._layer)
                : false;
        }

        public bool Equals(ILayer other)
        {
            return ((null != _layer) && (null != other))
                ? _layer.Equals(other)
                : false;
        }

        public bool Equals(string other)
        {
            return (null != _layer)
                ? _layer.Name.Equals(other)
                : false;
        }

        public override string ToString() 
        {
            return (null != _layer)
                ? _layer.Name
                : new StoLanguage() { AppName = Product.TechnicalAppname }.LoadStr(10090, "<no layer>");
        }
    }
}