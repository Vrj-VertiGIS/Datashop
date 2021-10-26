using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.ArcGIS.Gdb;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model.AddressSearch;
using GEOCOM.GNSDatashop.ServiceContracts;
using GEOCOM.GNSDatashop.Services.Config;

namespace GEOCOM.GNSDatashop.Services
{
    /// <summary>
    /// Service Class for the Address search
    /// </summary>
    public class AddressSearchService : IAddressSearchService
    {
        #region  Private Members

        private const string INITIALSEARCH = "initial";

        private const string FINALSEARCH = "final";

        private const double DISPLAYFACTOR = 0.1;

        private readonly AddressSearchConfig config = ServicesConfig.Instance.AddressSearch;

        private GeoSearch search;

        private GeoSearchDef searchDef;

        private List<FindControl> findControls;

        // log4net
        private static IMsg _log = new Msg(typeof (AddressSearchService));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressSearchService"/> class.
        /// </summary>
        public AddressSearchService()
        {
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.Server);
        }

        #endregion

        #region IAddressSearchService Methods

        /// <summary>
        /// Gets the search definitions.
        /// </summary>
        /// <returns></returns>
        public GeoFind GetSearchDefinitions()
        {
            return ServicesConfig.Instance.AddressSearch.GeoFind;
        }

        /// <summary>
        /// Performs the geo search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public GeoSearch PerformGeoSearch(GeoSearch search)
        {
            this.search = search;

            this.Find(search);

            return search;
        }

        /// <summary>
        /// Performs the result extent search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public List<ResultExtent> PerformResultExtentSearch(GeoSearch search)
        {
            var completedSearch = this.PerformGeoSearch(search);

            return completedSearch.QueryResult.ToList();
        }

        #endregion

        #region Private Methods

        private void Find(GeoSearch search)
        {
            if (search == null) throw new Exception("Find with empty search param");

            searchDef = GetSearchDef(search.Name);
            if (searchDef == null)
                throw new Exception(string.Format("Search {0:s} not found in configuration", search.Name));

            // is it the final 'zoom to' search
            if (search.RequestControlName.Equals(FINALSEARCH, StringComparison.InvariantCultureIgnoreCase))
            {
                _log.Debug("Final search called");
                // Fill combobox with queryresult
                string sqlQuery = parseFormula(searchDef.Select.Sql);
                sqlQuery = RemoveOptionalParameters(sqlQuery);
                _log.Debug("SQL query is " + sqlQuery);

                // Get Queryresults

                List<ResultExtent> extents = new List<ResultExtent>();

                // specialcase for coordinates search
                if (sqlQuery.StartsWith("RETURN", StringComparison.InvariantCultureIgnoreCase))
                {
                    extents.Add(GetNonSqlQueryLocation(sqlQuery));
                }
                else
                {
                    var results = EvaluateSqlQuery(sqlQuery);

                    foreach (IdTextPair result in results)
                    {
                        extents.Add(FindLocation(result.Id, sqlQuery));
                    }
                }

                _log.DebugFormat("Query returned {0} results", extents.Count);

                // return results
                search.QueryResult = extents.ToArray();
                return;
            }

            // is it the final 'zoom to' search
            if (!search.RequestControlName.Equals(INITIALSEARCH, StringComparison.InvariantCultureIgnoreCase))
            {
                SearchControl requestCtrl = GetCtrl(search.RequestControlName);
                if (requestCtrl == null)
                    throw new Exception(string.Format("Control {0:s} not found in parameters",
                                                      search.RequestControlName));
                SearchControlDef requestCtrlDef = GetControlDef(search.RequestControlName);
                if (requestCtrlDef == null)
                    throw new Exception(string.Format("Control {0:s} not found in configuration",
                                                      search.RequestControlName));
            }

            try
            {
                findControls = new List<FindControl>();
                foreach (SearchControl ctrl in search.Controls)
                {
                    var ctrldef = GetControlDef(ctrl.Name);
                    FindControl control = new FindControl(ctrl, ctrldef);
                    findControls.Add(control);
                }

                // set useskey
                if (!search.RequestControlName.Equals(INITIALSEARCH, StringComparison.InvariantCultureIgnoreCase))
                {
                    // mark all sql with variables equal to requestcontrol
                    CheckUsesKey(search.RequestControlName);
                }
                else
                {
                    // mark all sql without variables
                    CheckUsesVars();
                }

                // parse formula
                foreach (FindControl control in findControls)
                {
                    if (string.IsNullOrEmpty(control.UsesKey))
                    {
                        //just leave it
                        control.control.QueryResult = null;
                    }
                    else
                    {
                        if (control.UsesKey == search.RequestControlName)
                        {
                            // Fill combobox with queryresult
                            string sqlQuery = parseFormula(control.controlDef.SqlQuery.Sql);
                            sqlQuery = RemoveOptionalParameters(sqlQuery);
                            control.control.QueryResult = EvaluateSqlQuery(sqlQuery);

                            // wif 2010_06_21
                            // autoselect index if just one result is returned
                            // TODO auch bereits abgefüllte wieder behandeln
                            if (control.control.QueryResult.Length == 1)
                            {
                                ReplaceUsesKey(control.control.Name, search.RequestControlName);
                                control.control.Key = control.control.QueryResult[0].Id;
                            }
                        }
                        else
                        {
                            // Clear combobox
                            control.control.QueryResult = new IdTextPair[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Find {0:s}", ex.Message), ex);
                throw;
            }
        }

        private ResultExtent GetNonSqlQueryLocation(string query)
        {
            var parts = query.Substring(6).Split(',');
            double x, y;

            // default
            var scale = searchDef.Select.Scale;

            if (parts.Length >= 3)
                if (!double.TryParse(parts[2], out scale))
                    throw new Exception(string.Format("Bad scale in querystring {0}", query));

            if (parts.Length >= 2)
            {
                if (!double.TryParse(parts[0], out x) || !double.TryParse(parts[1], out y))
                    throw new Exception(string.Format("Bad x or y in querystring {0}", query));

                var width = (int) (scale*DISPLAYFACTOR/2);

                return new ResultExtent
                            {
                                X = x - width, 
                                Y = y - width, 
                                Width = 2*width, 
                                Height = 2*width
                            };
            }

            throw new Exception(string.Format("Bad querystring {0}", query));
        }

        /// <summary>
        /// get the coordinates from feature 
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private ResultExtent FindLocation(string objectid, string query)
        {
            try
            {
                int oid = Convert.ToInt32(objectid);

                // get table from sql
                string select;
                string from;
                string where;
                string orderField;
                bool orderDesc;
                SplitSqlQuery(query, out select, out from, out where, out orderField, out orderDesc);

                // if multiple tables are configured, take first
                var table = from.Split(',');

                IFeatureClass fc;
                IWorkspace ws;

                if (!string.IsNullOrEmpty(searchDef.DbConnection))
                    ws = WorkspaceUtils.OpenWorkspace(searchDef.DbConnection);
                else
                    throw new Exception("Missing db connection in configuration file");


                if (ws is IFeatureWorkspace)
                {
                    _log.DebugFormat("Find location for featureclass {0}", table[0]);

                    if ((ws as IWorkspace2).get_NameExists(esriDatasetType.esriDTFeatureClass, table[0]) == false)
                    {
                        _log.ErrorFormat("Featureclass {0} not found on arcgis server", table[0]);
                        return null;
                    }

                    IFeatureWorkspace fws = ws as IFeatureWorkspace;
                    _log.DebugFormat("Try opening {0}...", table[0]);
                    fc = fws.OpenFeatureClass(table[0]);
                    //ServiceContext.Instance.GeoDb.FeatureWorkspace.OpenFeatureClass(table[0]);
                    _log.DebugFormat(".. done. Try getting OBJECTID {0}", oid);

                    Assert.NotNull(fc, "Featureclass {0} missing", table[0]);

                    IFeature fea = fc.GetFeature(oid);

                    if (fea != null)
                    {
                        IEnvelope extent;
                        switch (fc.ShapeType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                                var pt = fea.Shape as IPoint;
                                extent = this.GetExtentFromPoint(pt);
                                break;
                            case esriGeometryType.esriGeometryPolyline:
                                extent = this.GetSearchResultExtentFromFeature(fea);
                                break;
                            case esriGeometryType.esriGeometryPolygon:
                                extent = this.GetSearchResultExtentFromFeature(fea);
                                break;
                            default:
                                _log.Error(string.Format("Geometry type {0:s} not supported", fc.ShapeType));
                                return null;
                        }

                        _log.DebugFormat("Found feature ({0}. Zooms to extent {1} / {2} / {3} / {4}", extent.XMin,
                                            extent.YMin, extent.XMax, extent.YMax, fea.OID);

    
                        return new ResultExtent(extent.XMin, extent.YMin,extent.Width,extent.Height);
                    }
                    _log.ErrorFormat("No feature with objectid {0} found.", oid);

                    return new ResultExtent();
                }
                throw new Exception("ArcGis Server Error"); //TODO: I don't ever want to see code like this! if we get round to Datashop v6, this has to go!
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("FindLocation {0:s}", ex.Message), ex);
                
                return null;
            }
        }

       
        private IEnvelope GetSearchResultExtentFromFeature(IFeature feature)
        {
            var geometry4 = feature.Shape as IGeometry4; //NOTE: no support for IGeometry5 for some reason

            if (geometry4 == null)
                throw new Exception(string.Format("Cannot convert object of type: {0} to IGeometry4", feature.Shape.GetType().UnderlyingSystemType.Name));

            var centroid = new PointClass //NOTE: so we have to roll our own centroid
            {
                X = (geometry4.Envelope.XMin + geometry4.Envelope.XMax) / 2,
                Y = (geometry4.Envelope.YMin + geometry4.Envelope.YMax) / 2
            };

            return searchDef.Select.Scale > 0
                ? this.GetExtentFromPoint(centroid)
                : geometry4.Envelope;
        }

        private IEnvelope GetExtentFromPoint(IPoint point)
        {
            var width = (int)(searchDef.Select.Scale * DISPLAYFACTOR);

            var extent = new EnvelopeClass();
            extent.PutCoords(0, 0, width, width);
            extent.CenterAt(point);

            return extent;
        }

        private IdTextPair[] EvaluateSqlQuery(string query)
        {
            _log.DebugFormat("Evaluating final query with SQL: {0}", query);
            List<IdTextPair> result = new List<IdTextPair>();

            try
            {
                string select;
                string from;
                string where;
                string orderField;
                bool orderDesc;
                SplitSqlQuery(query, out select, out from, out where, out orderField, out orderDesc);

                IWorkspace ws = null;
                if (!string.IsNullOrEmpty(searchDef.DbConnection))
                    ws = WorkspaceUtils.OpenWorkspace(searchDef.DbConnection);
                else
                    throw new Exception("Missing db connection in configuration file");

                IQueryDef queryDef = (ws as IFeatureWorkspace).CreateQueryDef();
                queryDef.SubFields = select;
                queryDef.Tables = from;
                queryDef.WhereClause = where;

                ICursor cursor = queryDef.Evaluate();

                if (cursor.Fields.FieldCount < 1)
                {
                    _log.WarnFormat("Fields are missing in query with SQL {0}", select);
                    Marshal.ReleaseComObject(cursor);
                    return result.ToArray();
                }

                if (!string.IsNullOrEmpty(orderField))
                {
                    _log.DebugFormat("Orderfield: {0}", orderField);
                    _log.DebugFormat("Descending: {0}", orderDesc);
                }


                // The last field is used as the key
                int iCode = cursor.Fields.FieldCount - 1;

                IRow row;
                while ((row = cursor.NextRow()) != null)
                {
                    IdTextPair itp = new IdTextPair();
                    itp.Text = "";
                    int i = 0;
                    do
                        //All fields except for the last one are concatenated and used as label. If only one field. it is used as key and label
                    {
                        itp.Text += getStrValue(row.get_Value(i));
                        i++;
                    } while (i < iCode);

                    i = row.Fields.FindField(orderField);
                    if (i >= 0)
                    {
                        itp.OrderValue = getStrValue(row.get_Value(i));
                        itp.IsText = (row.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeString);
                    }
                    else
                    {
                        itp.OrderValue = itp.Text;
                        itp.IsText = true;
                    }

                    itp.Id = getStrValue(row.get_Value(iCode));
                    itp.OrderDesc = orderDesc;

                    result.Add(itp);
                }

                _log.DebugFormat("Found {0} results in Query", result.Count);

                result.Sort();

                Marshal.ReleaseComObject(cursor);

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("evaluateSqlQuery {0} as user {1}", ex.Message, Environment.UserName), ex);
            }

            return result.ToArray();
        }

        private void SplitSqlQuery(string query, out string select, out string from, out string where,
                                   out string orderField, out bool orderDesc)
        {
            int pOrderBy = query.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase);
            orderDesc = false;

            if (pOrderBy > -1)
            {
                // extract order field
                orderField = query.Substring(pOrderBy + 8).Trim();

                // remove DESC
                int pDesc = orderField.IndexOf("DESC", StringComparison.CurrentCultureIgnoreCase);
                if (pDesc > -1)
                {
                    orderDesc = true;
                    orderField = orderField.Substring(0, pDesc).Trim();
                }

                // remove order by command
                query = query.Substring(0, pOrderBy).Trim();
            }
            else
            {
                orderField = string.Empty;
            }

            int pSelect = query.IndexOf("SELECT", StringComparison.CurrentCultureIgnoreCase);
            int pFrom = query.IndexOf("FROM", StringComparison.CurrentCultureIgnoreCase);
            int pWhere = query.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);

            select = query.Substring(pSelect + 6, pFrom - pSelect - 6).Trim();
            if (pWhere > 0)
            {
                from = query.Substring(pFrom + 4, pWhere - pFrom - 4).Trim();
                where = query.Substring(pWhere + 5).Trim();
            }
            else
            {
                from = query.Substring(pFrom + 4).Trim();
                where = "";
            }
        }

        private string getStrValue(object value)
        {
            if (value != null) return value.ToString();
            return "";
        }

        private string parseFormula(string formula)
        {
            StringBuilder sb = new StringBuilder();
            var parts = formula.Split('@');

            // append part before first @
            sb.Append(parts[0]);

            for (int i = 1; i < parts.Length - 1; i += 2)
            {

                if (i < parts.Length - 1)
                {
                    string var = parts[i];
                    SearchControl ctrl = GetCtrl(var);
                    if (ctrl != null)
                    {
                        // substitute var
                        if (!string.IsNullOrEmpty(ctrl.Key))
                        {
                            sb.Append(ctrl.Key);
                        }
                        else
                        {
                            // Mark as missing value for optional parameters {AND fld=..}
                            sb.Append('@');
                        }

                        // append next part
                        sb.Append(parts[i + 1]);
                    }
                }
                else
                {
                    _log.ErrorFormat("parseFormula: Formula {0:s} is not valid", formula);
                }

            }
            return sb.ToString();
        }

        private string RemoveOptionalParameters(string formula)
        {
            StringBuilder sb = new StringBuilder();
            var parts = formula.Split(new[] { '{', '}' });

            // append part before first @
            sb.Append(parts[0]);

            for (int i = 1; i < parts.Length - 1; i += 2)
            {
                if (i < parts.Length - 1)
                {
                    string optionalWhereSection = parts[i];
                    if (!optionalWhereSection.Contains("@")) //marker for empty param 
                        sb.Append(optionalWhereSection);

                    // append next part
                    sb.Append(parts[i + 1]);
                }
                else
                {
                    _log.ErrorFormat("removeOptionalParameters: Formula {0:s} is not valid", formula);
                }

            }
            return sb.ToString().Replace("@", "");
        }

        private void CheckUsesKey(string keyID)
        {
            string keyID2 = string.Format("@{0:s}@", keyID);
            foreach (FindControl control in findControls)
            {
                if (string.IsNullOrEmpty(control.UsesKey))
                {
                    if (control.controlDef != null && control.controlDef.SqlQuery != null)
                    {
                        string sql = control.controlDef.SqlQuery.Sql;
                        if (sql != null && sql.Contains(keyID2))
                        {
                            control.UsesKey = keyID;
                            CheckUsesKey(control.controlDef.Name);
                        }
                    }
                }
            }
        }

        private void ReplaceUsesKey(string controlID, string searchID)
        {
            foreach (FindControl control in findControls)
            {
                if (!string.IsNullOrEmpty(control.UsesKey) &&
                    control.UsesKey.Equals(controlID, StringComparison.InvariantCultureIgnoreCase))
                {
                    control.UsesKey = searchID;
                }
            }
        }

        private void CheckUsesVars()
        {
            foreach (FindControl control in findControls)
            {
                if ((control.controlDef != null) && (control.controlDef.SqlQuery != null))
                {
                    string sql = control.controlDef.SqlQuery.Sql;
                    if (sql != null && !sql.Contains("@")) // TODO Emailadresse oder ähnliches in Query erlauben
                    {
                        // Fill with values
                        control.UsesKey = INITIALSEARCH;
                    }
                    else
                    {
                        // returns an empty array
                        control.UsesKey = "_EMPTY";
                    }
                }
                else
                {
                    // returns an empty array
                    control.UsesKey = "_EMPTY";
                }
            }
        }

        #endregion

        private static bool CheckAndInitializeLicense(IAoInitialize engineInitialize, esriLicenseProductCode productCode)
        {
            esriLicenseStatus licenseStatus = engineInitialize.IsProductCodeAvailable(productCode);

            if (licenseStatus == esriLicenseStatus.esriLicenseAlreadyInitialized)
                return true;

            if (licenseStatus == esriLicenseStatus.esriLicenseAvailable)
            {
                esriLicenseStatus status = engineInitialize.Initialize(productCode);

                return status == esriLicenseStatus.esriLicenseCheckedOut;
            }

            return false;
        }

        private SearchControl GetCtrl(string id)
        {
            foreach (SearchControl ctrl in search.Controls)
            {
                if (ctrl.Name != null && ctrl.Name.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                    return ctrl;
            }
            return null;
        }

        private SearchControlDef GetControlDef(string id)
        {
            foreach (SearchControlDef ctrl in searchDef.Controls)
            {
                if (ctrl.Name != null && ctrl.Name.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                    return ctrl;
            }
            return null;
        }

        private GeoSearchDef GetSearchDef(string id)
        {
            foreach (GeoSearchDef searchDef in config.GeoFind.Searches)
            {
                if (searchDef != null && searchDef.Name != null &&
                    searchDef.Name.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                    return searchDef;
            }

            return null;
        }

    }
}