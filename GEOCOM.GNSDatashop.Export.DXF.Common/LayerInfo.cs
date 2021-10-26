using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class LayerInfo : IEquatable<LayerInfo>, IComparable<LayerInfo>
    {
        public string LayerName { get; private set; }
        public enum LayerTypeEnum { dimension, annotation, basemap, raster, feature, representation, drawing, any, none };
        public LayerTypeEnum LayerType { get; private set; }
        public bool IsSupported { get; set; }
        public esriGeometryType GeometryType { get; private set; }
        public string LayerDefinitionExpression { get; private set; }
        public bool Valid { get; private set; }

        public LayerInfo(ILayer layer)
        {
            LayerName = layer.Name;
            Valid = layer.Valid;
            LayerType = TypeFromLayer(layer);
            LayerDefinitionExpression = (layer is IFeatureLayerDefinition fld) ? fld.DefinitionExpression : string.Empty;
            GeometryType = GeometryTypeFromLayer(layer);

            IsSupported = true;
        }

        public LayerInfo(IEnumerable<IElement> drawingLayer)
        {
            LayerName = "ArcMAP document drawing layer";
            LayerType = LayerTypeEnum.drawing;
            LayerDefinitionExpression = string.Empty;
            GeometryType = esriGeometryType.esriGeometryAny;
            IsSupported = true;
        }

        private LayerTypeEnum TypeFromLayer(ILayer layer)
            => (null == layer) ? LayerTypeEnum.none
                : layer is IDimensionLayer ? LayerTypeEnum.dimension
                : layer is IBasemapLayer ? LayerTypeEnum.basemap
                : layer is IRasterLayer ? LayerTypeEnum.raster
                : null != ((layer as IGeoFeatureLayer)?.Renderer as IRepresentationRenderer) ? LayerTypeEnum.representation
                : layer is IFeatureLayer fl && (null != fl.FeatureClass)    // feature layer? Valid db connection (null != featureClass)?
                    ? fl.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation
                        ? LayerTypeEnum.annotation
                        : fl.FeatureClass.FeatureType == esriFeatureType.esriFTDimension
                            ? LayerTypeEnum.dimension
                            : LayerTypeEnum.feature
                : LayerTypeEnum.any;

        private esriGeometryType GeometryTypeFromLayer(ILayer layer)
            => (null != layer) && (layer is IFeatureLayer flyr)
                ? flyr?.FeatureClass?.ShapeType ?? esriGeometryType.esriGeometryAny
                : esriGeometryType.esriGeometryAny;

        #region IComparable<ErroneousLayerInfo>
        public int CompareTo(LayerInfo other)
            => ByLayerComparer.Compare(this, other);
        #endregion
        #region IEquatable<ErroneousLayerInfo>
        public bool Equals(LayerInfo other)
            => ByLayerEqualityComparer.Equals(this, other);
        #endregion

        #region Icomparable/IEquatable support
        public static IComparer<LayerInfo> ByLayerComparer => new LayerComparer();
        public static IEqualityComparer<LayerInfo> ByLayerEqualityComparer => new LayerComparer();
        public class LayerComparer : IComparer<LayerInfo>, IEqualityComparer<LayerInfo>
        {
            public bool Equals(LayerInfo x, LayerInfo y)
                => (!string.IsNullOrEmpty(x.LayerName))
                    ? (!string.IsNullOrEmpty(y.LayerName))
                        ? x.LayerName.Equals(y.LayerName)
                        : false
                    : string.IsNullOrEmpty(y.LayerName);

            public int Compare(LayerInfo x, LayerInfo y)
                => !string.IsNullOrEmpty(x.LayerName)
                    ? !string.IsNullOrEmpty(y.LayerName)
                        ? x.LayerName.CompareTo(y.LayerName)
                        : 1
                    : !string.IsNullOrEmpty(y.LayerName)
                        ? 1
                        : 0;

            public int GetHashCode(LayerInfo obj)
                => obj.LayerName.GetHashCode();
        }

        #endregion
    }
}
