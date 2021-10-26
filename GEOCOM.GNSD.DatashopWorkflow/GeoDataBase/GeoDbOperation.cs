using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.Attributes;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DatashopWorkflow.IntersectionData;
using IntersectData = GEOCOM.GNSD.DatashopWorkflow.IntersectionData.IntersectionData;
using System.Linq;

namespace GEOCOM.GNSD.DatashopWorkflow.GeoDataBase
{
    public static class GeoDbOperation
    {
        static readonly IMsg logger = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        public static IPoint GetCenterPoint(List<IGeometry> extentList, CenterAreaType centerAreaType)
        {
            IGeometry unionGeometry = UnionGeometries(extentList);
            IArea area = unionGeometry as IArea;

            if (area == null)
                return new Point();

            switch (centerAreaType)
            {
                case CenterAreaType.CenterPoint:
                    return area.Centroid;

                case CenterAreaType.LabelPoint:
                    return area.LabelPoint;

                default:
                    return new Point();
            }
        }

        public static Dictionary<string, string> ExtractIntersectionData(ExtractionInfo extractionConfig, List<IGeometry> jobExtents)
        {
            var extractionData = new Dictionary<string, string>();
            IGeometry multipartPolygon = UnionGeometries(jobExtents);

            if (extractionConfig == null || extractionConfig.ExtractionItems == null)
            {
                logger.Error("No extraction items found.");
                return extractionData;
            }

            foreach (var extractionItem in extractionConfig.ExtractionItems)
            {
                logger.DebugFormat(
                    "Extracting data for FeatureClass={0}, SourceColumn={1}, DestinationColumn={2}, IntersectionType={3}",
                    extractionItem.FeatureClass,
                    extractionItem.SourceColumn,
                    extractionItem.DestinationColumn,
                    extractionItem.IntersectionType);

                IntersectData intersecData = GetIntersectingObjects(
                                                            extractionConfig.WorkspaceConnection,
                                                            extractionItem,
                                                            multipartPolygon);

                string destColumn = extractionItem.DestinationColumn;
                switch (extractionItem.IntersectionType)
                {
                    case IntersectionType.MaxIntersectionArea:
                        string nameOfBiggestIntersection = intersecData.GetNameOfBiggestIntersection();
                        extractionData[destColumn] = nameOfBiggestIntersection;
                        break;

                    case IntersectionType.MaxIntersectionLength:
                        goto case IntersectionType.MaxIntersectionArea;

                    case IntersectionType.AllIntersectingObjects:
                        string allElementsNameSorted = intersecData.GetAllElementsNameSorted(extractionItem.Separator);
                        extractionData[destColumn] = allElementsNameSorted;
                        break;
                }
            }

            return extractionData;
        }

        public static void WriteJobExtents(ExtentDataBaseInfo extentDataBaseConfig, List<IGeometry> jobExtents, long jobId)
        {
            JobExtentInfo jobExtentInfoCfg = extentDataBaseConfig.JobExtentInfo;
            IFeatureWorkspace extentWorkspace = OpenWorkspace(extentDataBaseConfig.Path);

            var workspaceEdit = extentWorkspace as IWorkspaceEdit2;

            try
            {
                var multiuserWorkspaceEdit = extentWorkspace as IMultiuserWorkspaceEdit;

                if (multiuserWorkspaceEdit != null)
                {
                    bool supportsNonVersionEditing =
                        multiuserWorkspaceEdit.SupportsMultiuserEditSessionMode(
                            esriMultiuserEditSessionMode.esriMESMNonVersioned);
                    if (!supportsNonVersionEditing)
                        throw new Exception("The workspace does not support non-versioned editing.");

                    multiuserWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);
                }

                IFeatureClass featureClass = OpenFeatureClass(extentWorkspace, jobExtentInfoCfg.FeatureClass);

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("{0} = {1}", jobExtentInfoCfg.ColJobId, jobId);

                // Delete previously stored extents for this jobs
                IFeatureCursor cursor = featureClass.Search(queryFilter, false);
                var extentFeature = cursor.NextFeature();
                while (extentFeature != null)
                {
                    extentFeature.Delete();
                    extentFeature = cursor.NextFeature();
                }
                Marshal.ReleaseComObject(cursor);

                int fieldIndex = FindFieldIndex(featureClass.Fields, jobExtentInfoCfg.ColJobId);
                foreach (IGeometry jobExtent in jobExtents)
                {
                    IFeature feature = featureClass.CreateFeature();
                    IRowSubtypes rowSubtypes = feature as IRowSubtypes;
                    if (rowSubtypes != null)
                        rowSubtypes.InitDefaultValues();

                    feature.Value[fieldIndex] = jobId;
                    feature.Shape = jobExtent;

                    feature.Store();
                }
            }
            finally
            {
                if (workspaceEdit != null)
                    workspaceEdit.StopEditing(true);

                TryCleanOracleKeySetTables(extentWorkspace);
            }
        }

        /// <summary>
        /// Attemp to clean Oracle keyset tables.
        /// </summary>
        /// <remarks>
        /// Keyset tables are temporary table created for edit session. In Oracle sometimes they stay in the DB and are not cleaned automatically by ArcObjects. 
        /// </remarks>
        /// <seealso cref="http://support.esri.com/en/technical-article/000009802"/>
        /// <seealso cref="https://issuetracker02.eggits.net/browse/DATASHOP-618"/>
        /// <param name="workspace">Database workspace</param>
        private static void TryCleanOracleKeySetTables(IFeatureWorkspace workspace)
        {
            try
            {
                var isOracleDmbs = workspace is IWorkspace && workspace is IDatabaseConnectionInfo2 && ((IDatabaseConnectionInfo2)workspace).ConnectionDBMS == esriConnectionDBMS.esriDBMS_Oracle;
                if (!isOracleDmbs) return;

                // following statement is adjusted statement (for user with lower privilegies) from http://support.esri.com/en/technical-article/000009802
                // it drops all Keyset* table of the current user
                var dropKeysetsTablesSql =
                    @"
DECLARE
CURSOR all_keysets IS
SELECT owner, table_name 
FROM all_tables
WHERE table_name LIKE 'KEYSET_%' AND owner IN (SELECT sys_context ('userenv', 'session_user') FROM dual);

BEGIN
    FOR drop_keysets IN all_keysets LOOP
	    EXECUTE IMMEDIATE 'DROP TABLE '||drop_keysets.owner||'.'||drop_keysets.table_name;
    END LOOP;
END;
";
                ((IWorkspace)workspace).ExecuteSQL(dropKeysetsTablesSql);
            }
            catch (Exception e)
            {
                logger.Error("Could not clean up Oracle keyset tables.", e);
            }
        }

        /// <summary>
        /// Look up affected data owners (owners who supply data which has been
        /// plotted in the current job. 
        /// </summary>
        /// <param name="notificationDbInfo">The notification database info</param>
        /// <param name="jobExtents">The job extents to check</param>
        /// <returns>A list of affected data owners who intersect with the job extent</returns>
        public static List<AffectedDataOwner> GetAffectedDataOwners(NotificationDataBaseInfo notificationDbInfo, List<IGeometry> jobExtents)
        {
            var jobExtent = UnionGeometries(jobExtents);
            var notifyExtentOwnerInfoList = new List<AffectedDataOwner>();
            var ownerExtentGroups = new Dictionary<int?, List<ExtentData>>();

            var dataOwnerExtentInfo = notificationDbInfo.DataOwnerExtentInfo;
            var workspace = OpenWorkspace(notificationDbInfo.Path);
            var featureClass = OpenFeatureClass(workspace, dataOwnerExtentInfo.FeatureClass);

            var spatialFilter = new SpatialFilterClass
                {
                    Geometry = jobExtent,
                    GeometryField = featureClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                };

            IFeatureCursor extentsCursor = featureClass.Search(spatialFilter, true);
            int idOwnerId = FindFieldIndex(featureClass.Fields, dataOwnerExtentInfo.ColOwnerId);
            int idDescription = FindFieldIndex(featureClass.Fields, dataOwnerExtentInfo.ColExtentDescription);

            for (IFeature ownerExtent = extentsCursor.NextFeature();
                  ownerExtent != null;
                  ownerExtent = extentsCursor.NextFeature())
            {
                var extentData = new ExtentData(ownerExtent.Value[idOwnerId],
                                                ownerExtent.OID,
                                                ownerExtent.Shape,
                                                ownerExtent.Value[idDescription]);

                if (extentData.OwnerId == null)
                    continue;

                if (!ownerExtentGroups.ContainsKey(extentData.OwnerId))
                    ownerExtentGroups[extentData.OwnerId] = new List<ExtentData>();

                ownerExtentGroups[extentData.OwnerId].Add(extentData);
            }

            foreach (KeyValuePair<int?, List<ExtentData>> group in ownerExtentGroups)
            {
                DataOwner owner = null;
                if (group.Key != null)
                    owner = GetDataOwnerDataById(notificationDbInfo, group.Key.Value);

                if (owner != null)
                    notifyExtentOwnerInfoList.Add(new AffectedDataOwner(owner, group.Value));
            }

            Marshal.ReleaseComObject(extentsCursor);

            return notifyExtentOwnerInfoList;
        }

        /// <summary>
        /// Finds features intersecting with the job extents.
        /// </summary>
        /// <param name="jobExtents">The job extents.</param>
        /// <param name="workspacePath">The workspace path.</param>
        /// <param name="featureClassName">Name of the feature class.</param>
        /// <returns>
        /// File paths and feature id.
        /// </returns>
        public static IFeature[] FindIntersectingFeatures(List<IGeometry> jobExtents, string workspacePath, string featureClassName)
        {
            IGeometry unionGeometries = UnionGeometries(jobExtents);
            var workspace = OpenWorkspace(workspacePath);
            var featureClass = OpenFeatureClass(workspace, featureClassName);
            IFeature[] geoAttachmentsFeatures = FindIntersectingFeatures(featureClass, unionGeometries).ToArray();
            return geoAttachmentsFeatures;
        }

        #region ExtractIntersectionData methods

        private static IntersectData GetIntersectingObjects(string workspaceConnection, ExtractionItemElementInfo extractionItem, IGeometry jobExtent)
        {
            var intersectionData = new IntersectionData.IntersectionData();

            var workspace = OpenWorkspace(workspaceConnection);
            var featureClass = OpenFeatureClass(workspace, extractionItem.FeatureClass);

            CheckIntersectionTypeAndGeometryTypeMatch(extractionItem.IntersectionType, featureClass.ShapeType);

            int fieldIndex = FindFieldIndex(featureClass.Fields, extractionItem.SourceColumn);

            IEnumerable<IFeature> itersectingFeatures = FindIntersectingFeatures(featureClass, jobExtent);
            foreach (var intersectionFeatures in itersectingFeatures)
            {
                ITopologicalOperator2 topologicalOperator = jobExtent as ITopologicalOperator2;
                if (topologicalOperator == null)
                    continue;

                IGeometry intersectGeometry = topologicalOperator.Intersect(intersectionFeatures.ShapeCopy, intersectionFeatures.Shape.Dimension);

                string intersectionName = (intersectionFeatures.Value[fieldIndex] ?? "null").ToString();
                IntersectionGeometry intersectionValue = null;
                switch (intersectGeometry.GeometryType)
                {
                    case esriGeometryType.esriGeometryPolygon:
                        IArea intersectionArea = intersectGeometry as IArea;
                        if (intersectionArea != null)
                            intersectionValue = new IntersectionArea(intersectionName, intersectionArea.Area);
                        break;

                    case esriGeometryType.esriGeometryPolyline:
                        IPolyline intersectionPolyline = intersectGeometry as IPolyline;
                        if (intersectionPolyline != null)
                            intersectionValue = new IntersectionPolyline(intersectionName, intersectionPolyline.Length);
                        break;

                    case esriGeometryType.esriGeometryPoint:
                        IPoint intersectionPoint = intersectGeometry as IPoint;
                        if (intersectionPoint != null)
                            intersectionValue = new IntersectionPoint(intersectionName);
                        break;
                }

                if (intersectionValue == null)
                {
                    string message = string.Format("The map extent intersects with the specified extraction feature class. However the intersection resulted in an unkown geometry type of '{0}'.", intersectGeometry.GeometryType.ToString());
                    throw new ArgumentException(message);
                }

                intersectionData.Add(intersectionValue);
            }

            return intersectionData;
        }

        /// <summary>
        /// Check integrity of the geometry type and the intersection extraction operation.
        /// </summary>
        private static void CheckIntersectionTypeAndGeometryTypeMatch(IntersectionType intersectionType, esriGeometryType geometryType)
        {
            bool match = false;
            switch (intersectionType)
            {
                case IntersectionType.MaxIntersectionArea:
                    match = geometryType == esriGeometryType.esriGeometryPolygon;
                    break;
                case IntersectionType.MaxIntersectionLength:
                    match = geometryType == esriGeometryType.esriGeometryPolyline;
                    break;
                case IntersectionType.AllIntersectingObjects:
                    match = geometryType == esriGeometryType.esriGeometryPolyline ||
                            geometryType == esriGeometryType.esriGeometryPolygon ||
                            geometryType == esriGeometryType.esriGeometryPoint;
                    break;
            }

            if (!match)
                throw new ArgumentException(string.Format("Intersection type '{0}' and geometry type '{1}' does not match . Please review the extraction configuration.", intersectionType, geometryType));
        }

        #endregion

        #region GetAffectedDataOwners methods

        /// <summary>
        /// Load data owner info from GDB and create data container
        /// object instance.
        /// </summary>
        /// <param name="notificationDataBaseInfo">The notofiaction base data info</param>
        /// <param name="id">OBJECTID of data owner table IRow</param>
        /// <returns>DataOwnerInfo instance if row valid, null otherwise</returns>
        private static DataOwner GetDataOwnerDataById(NotificationDataBaseInfo notificationDataBaseInfo, int id)
        {
            var workspace = OpenWorkspace(notificationDataBaseInfo.Path);
            var table = OpenTable(workspace, notificationDataBaseInfo.DataOwnerInfo.Table);

            var row = GetRowById(table, id);
            var dataOwner = ConvertToDataOwnerData(notificationDataBaseInfo, row);
            return dataOwner;
        }

        /// <summary>
        /// Get a row from a given table
        /// </summary>
        /// <param name="table">Table (AO interface ITable) to get from</param>
        /// <param name="id">OBJECTID to get data row of</param>
        /// <returns>IRow interface to data row upon success, null otherwise</returns>
        private static IRow GetRowById(ITable table, int id)
        {
            try
            {
                return table.GetRow(id);
            }
            catch (COMException e)
            {
                var rowNotFound = (uint)e.ErrorCode == 0x80040952;
                if (rowNotFound)
                    return null;

                throw;
            }
        }

        /// <summary>
        /// Extract the info to a data owner from the related table row (IRow)
        /// and create a data container object (DataOwnerInfo) instance.
        /// </summary>
        /// <param name="notificationDataBaseInfo">The notofication base data info</param>
        /// <param name="row">IRow to be read</param>
        /// <returns>DataOwnerInfo instance if row valid, null otherwise</returns>
        private static DataOwner ConvertToDataOwnerData(NotificationDataBaseInfo notificationDataBaseInfo, IRow row)
        {
            if (row == null)
                return null;

            DataOwnerInfo dataOwnerInfoCfg = notificationDataBaseInfo.DataOwnerInfo;

            // for handyness
            IFields flds = row.Fields;


            object email = row.Value[flds.FindField(dataOwnerInfoCfg.ColEmail)];
            object desc = row.Value[flds.FindField(dataOwnerInfoCfg.ColDescription)];
            int ownerId = row.OID;

            return new DataOwner(
                                  ownerId,
                                  (email != DBNull.Value) ? email.ToString() : null,
                                  (desc != DBNull.Value) ? desc.ToString() : null);
        }

        #endregion

        #region Common utility methods

        private static IEnumerable<IFeature> FindIntersectingFeatures(IFeatureClass featureClass, IGeometry jobExtent)
        {
            if (featureClass == null || jobExtent == null)
                yield break;

            var spatialFilter = new SpatialFilterClass
                {
                    Geometry = jobExtent,
                    GeometryField = featureClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                };

            IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);

            for (IFeature nextFeature = featureCursor.NextFeature();
                 nextFeature != null;
                 nextFeature = featureCursor.NextFeature())
            {
                ITopologicalOperator2 topologicalOperator = jobExtent as ITopologicalOperator2;
                if (topologicalOperator == null)
                    continue;

                yield return nextFeature;
            }

            Marshal.ReleaseComObject(featureCursor);
        }

        /// <summary>
        /// Creates an union of geometries.
        /// </summary>
        /// <param name="geometries">The geometries.</param>
        /// <returns>Union of the geometries</returns>
        private static IGeometry UnionGeometries(List<IGeometry> geometries)
        {
            var unionedPolygon = new PolygonClass();
            if (geometries == null || geometries.Count == 0)
                return unionedPolygon;

            var geometryBag = new GeometryBagClass();
            geometryBag.SpatialReference = geometries.First().SpatialReference;

            foreach (IGeometry geometry in geometries)
            {
                object missing = Type.Missing;
                geometryBag.AddGeometry(geometry, ref missing, ref missing);
            }

            unionedPolygon.ConstructUnion(geometryBag);
            return unionedPolygon;
        }

        public static IFeatureWorkspace OpenWorkspace(string path)
        {
            try
            {
                var featureWorkspace = Utils.Utils.OpenWorkspace(path) as IFeatureWorkspace;
                if (featureWorkspace == null)
                    throw new ArgumentException("The workspace '{0}' could not be open.", path);

                var version = featureWorkspace as IVersion;
                if (version != null)
                    version.RefreshVersion();

                return featureWorkspace;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("The workspace '{0}' could not be open.", e, path);
                throw;
            }
        }

        public static IFeatureClass OpenFeatureClass(IFeatureWorkspace workspace, string featureClassName)
        {
            try
            {
                IFeatureClass featureClass = workspace.OpenFeatureClass(featureClassName);
                if (featureClass == null)
                    throw new ArgumentException(string.Format("Feature class '{0}', could not be open.", featureClassName));

                return featureClass;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Feature class '{0}', could not be open.", e, featureClassName);
                throw;
            }
        }

        private static ITable OpenTable(IFeatureWorkspace workspace, string tableName)
        {
            try
            {
                ITable table = workspace.OpenTable(tableName);
                if (table == null)
                    throw new ArgumentException(string.Format("Table '{0}' could not be open.", tableName));

                return table;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Table '{0}' could not be open.", e, tableName);
                throw;
            }
        }

        /// <summary>
        /// Finds the index of the field with name in the <paramref name="fieldName"/>.
        /// </summary>
        /// <param name="fields">The fields collection.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Index of the field</returns>
        /// <exception cref="System.ArgumentException">If the field could not be found.</exception>
        public static int FindFieldIndex(IFields fields, string fieldName)
        {
            try
            {
                int fieldIndex = fields.FindField(fieldName);

                bool fieldFound = fieldIndex >= 0;
                if (!fieldFound)
                    throw new ArgumentException(
                        string.Format("Field '{0}' could not be found.", fieldName));

                return fieldIndex;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Field '{0}' could not be found.", e, fieldName);
                throw;
            }
        }

        #endregion
    }
}
