using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.Common.Clipping;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using log4net;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class ESRIFeatureList 
    {
        public enum CoordinateDim { OneD, TwoD, ThreeD, Unknown }

        private const bool _RecyclingCursor = false;

        private static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        private ILayer _lyr = null;
        private RegionOfInterest _regionOfInterest = null;
        private bool _applySelectionSet = false;

        public ESRIFeatureList(ILayer lyr, bool applySelectionSet)
        {
            _lyr = lyr;

            _applySelectionSet = applySelectionSet;
        }

        public ESRIFeatureList(ILayer lyr, RegionOfInterest regionOfInterest, bool applySelectionSet) 
            : this(lyr, applySelectionSet)
        {
            _regionOfInterest = regionOfInterest;
        }

        public CoordinateDim CoordinateDimension
        {
            get { return GetCoordinateDimension(_lyr); }
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                try
                {
                    for (var feature = FeatureCursor.NextFeature(); (null != feature); feature = FeatureCursor.NextFeature())
                        yield return feature;
                }
                finally
                {
                    FeatureCursor = null;
                }
            }
        }

        public int Count
        {
            get
            {
                /// Use recycling cursors since the export will happen a row at a time with non-intermixed rows of the same
                /// cursor
                if (((_lyr is IFeatureSelection) && _applySelectionSet)
                    || (_lyr is IDisplayTable))
                    return Features.Count();
                else if (_lyr is IGeoFeatureLayer gfl)      // faster than iterating the features
                    return gfl.DisplayFeatureClass.FeatureCount(QueryFilter);
                else if (_lyr is IFeatureLayer fl)
                    return fl.FeatureClass.FeatureCount(QueryFilter);
                else
                    throw new Exception($"{_lyr.Name} cannot be searched for display features");
            }
        }

        private IFeatureCursor _featureCursor = null;
        private IFeatureCursor FeatureCursor
        {
            get => _featureCursor ?? (_featureCursor = PrepareFeatureCursor());
            set
            {
                if (null != value)
                    throw new ArgumentException("Can set only to null");
                if (null != _featureCursor)
                    Marshal.ReleaseComObject(_featureCursor);
                _featureCursor = null;
            }
        }

        private IFeatureCursor PrepareFeatureCursor()
        {
            /// Use recycling cursors since the export will happen a row at a time with non-intermixed rows of the same
            /// cursor
            if ((_applySelectionSet) && (_lyr is IFeatureSelection fls))
            {
                ICursor cursor;
                fls.SelectionSet.Search(QueryFilter, _RecyclingCursor, out cursor);
                return cursor as IFeatureCursor;
            }
            else if ((_lyr is IDisplayTable dt) && !FeatureLinkedAnnotation)
                // Occasionally - in certain cases (reason unknown) there happen to be non-annotation features 
                // originating from a IDisplayTable.SearchDisplayTable() executed on a annotation feature layer.
                // So - by now - omit this "short circuit" for annotations. 
                return dt.SearchDisplayTable(QueryFilter, _RecyclingCursor) as IFeatureCursor;
            else if (_lyr is IGeoFeatureLayer gfl)
                return (gfl).SearchDisplayFeatures(QueryFilter, _RecyclingCursor);
            else if (_lyr is IFeatureLayer fl)
                return (fl).Search(QueryFilter, _RecyclingCursor);
            else
                throw new Exception($"{_lyr.Name} cannot be searched for display features");
        }

        private bool FeatureLinkedAnnotation
        {
            get
            {
                var fl = _lyr is IAnnotationLayer
                && (null != ((_lyr as IFeatureLayer)?.FeatureClass?.Extension as IAnnotationClassExtension).LinkedFeatureClass);
                return fl;
            }
        }

        private IQueryFilter _queryFilter = null;

        private IQueryFilter QueryFilter
        {
            get => _queryFilter ?? (_queryFilter = PrepareQueryFilter());
            set => _queryFilter = value;
        }

        private IQueryFilter PrepareQueryFilter()
            => ((null != _regionOfInterest) && (_regionOfInterest.Region != null) && !_regionOfInterest.Region.IsEmpty)
                ? CreateExtentSpatialFilter(_regionOfInterest) : null;

        #region private helpers

        private IQueryFilter CreateExtentSpatialFilter(RegionOfInterest regionOfInterest)
        {
            var sf = new SpatialFilterClass() as ISpatialFilter;
            regionOfInterest.PrepareQueryFilter(sf);
            if (_lyr is IFeatureLayer flyr)
                sf.GeometryField = flyr.FeatureClass.ShapeFieldName;
            else if (_lyr is IAnnotationSublayer alyr)
            {
                var fl = alyr.Parent as IFeatureLayer;
                sf.GeometryField = fl.FeatureClass.ShapeFieldName;
            }

            return sf;
        }

        private CoordinateDim GetCoordinateDimension(ILayer layer)
            => layer is IFeatureLayer flyr
                ? GetCoordinateDimension(flyr)
                : layer is IAnnotationSublayer slyr
                    ? GetCoordinateDimension(slyr.Parent as ILayer)
                    : CoordinateDim.TwoD;

        private CoordinateDim GetCoordinateDimension(IFeatureLayer  layer)
        {
            try
            {
                var featureClass = layer.FeatureClass;
                var shapeFieldIndex = featureClass.FindField(featureClass.ShapeFieldName);
                var shapeField = featureClass.Fields.Field[shapeFieldIndex];

                return (shapeField.GeometryDef.HasZ) ? CoordinateDim.ThreeD : CoordinateDim.TwoD;
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("Error retrieving geometry dimension from Layer '{0}'.GeometryDef\nException: {1}", layer.Name, ex.ToString()));
 
                return CoordinateDim.Unknown;
            }
        }

        #endregion
    }
}
