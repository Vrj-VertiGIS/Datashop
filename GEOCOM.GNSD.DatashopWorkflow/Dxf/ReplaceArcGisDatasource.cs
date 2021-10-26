using System;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.Common.Logging;

namespace GEOCOM.GNSD.DatashopWorkflow.Dxf
{
    public class ReplaceArcGisDatasource
    {
        // Logging
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        // Reference to target workspace
        private IWorkspace _targetWorkspace;

        public string Qualifier { get; set; }

        public ReplaceArcGisDatasource(IMsg log)
        {
            // replace logger with client logger
            _log = log;
        }

        private void replaceDatasourceForLayer(ILayer rootLayer)
        {
            if (rootLayer is IGroupLayer)
            {
                for (int i = 0; i < (rootLayer as ICompositeLayer).Count; i++)
                {

                    ILayer layer = (rootLayer as ICompositeLayer).get_Layer(i);
                    //Datenquelle umhängen
                    try
                    {
                        // Recursiv call to process group layer tree
                        replaceDatasourceForLayer(layer);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("ReplaceDatasourceForLayer {0:s}", ex.Message), ex);
                    }
                }
            }
            else
            {
                // simple layer
                changeDatasourceForLayer(rootLayer);
            }

        }

        private void QualifyDatasourceName(ref string dsName)
        {
            var s = dsName.Split('.');
            dsName = AddQualifierToName(s[s.Length - 1], Qualifier);
        }

        public static string AddQualifierToName(string tablename, string qualifier)
        {
            if (string.IsNullOrEmpty(qualifier))
                return tablename;
            else
                return qualifier + "." + tablename;
        }

        /// <summary>
        /// throws exception
        /// </summary>
        /// <param name="layer"></param>
        private void changeDatasourceForLayer(ILayer layer)
        {
            if (layer is IRasterLayer)
            {
                #region Rasterlayer

                IDataLayer2 dl = layer as IDataLayer2;
                //dl.Disconnect();
                IName dsn = dl.DataSourceName;
                if (dsn is IDatasetName)
                {
                    var rdsn = dsn as IDatasetName;

                    string dsName = rdsn.Name;
                    string usedDsName = dsName;
                    QualifyDatasourceName(ref usedDsName);

                    if (_targetWorkspace != null)
                    {
                        // get new raster
                        var rd = (_targetWorkspace as IRasterWorkspace).OpenRasterDataset(usedDsName);
                        (dl as IRasterLayer).CreateFromDataset(rd);

                        // TODO Check if Renderer has changed
                        _log.InfoFormat("Rasterdataset {0:s} changed to new datasource.", dsName);

                    }
                    else
                    {
                        _log.WarnFormat("Rasterdataset {0:s} not found in new datasource.", dsName);
                    }
                }
                else
                {
                    _log.WarnFormat("Rasterdataset {0:s} has an unknwown datasourcetype.", layer.Name);
                }

            }

                #endregion


            else if (layer is IRasterCatalogLayer)
            {
                #region RasterCatalogLayer

                // TODO

                #endregion
            }

            else if (layer is IGdbRasterCatalogLayer)
            {
                #region GdbRasterCatalogLayer


                #endregion
            }

            else if (layer is IFeatureLayer)
            {
                #region Featurelayer

                IDataLayer2 dl = layer as IDataLayer2;
                //dl.Disconnect();
                IName dsn = dl.DataSourceName;
                if (dsn is IDatasetName)
                {
                    var fdsn = dsn as IDatasetName;

                    string dsName = fdsn.Name;

                    string usedDsName = dsName;
                    QualifyDatasourceName(ref usedDsName);

                    if (_targetWorkspace != null)
                    {
                        // get New Table
                        _log.InfoFormat("Opening featureclass {0}.", usedDsName);
                        var fc = (_targetWorkspace as IFeatureWorkspace).OpenFeatureClass(usedDsName);
                        (dl as IFeatureLayer).FeatureClass = fc;
                        _log.InfoFormat("Featureclass {0:s} changed to new datasource.", dsName);

                    }
                    else
                    {
                        _log.WarnFormat("Featureclass {0:s} not found in new datasource.", dsName);
                    }
                }
                else
                {
                    _log.WarnFormat("Featurelayer {0:s} has an unknwown datasourcetype.", layer.Name);
                }

                #endregion

            }
        }

        private string ReplaceOwner(string name, string owner)
        {
            string[] parts = name.Split('.');
            switch (parts.Length)
            {
                case 1:
                    return string.Format("{0:s}.{1:s}", owner, name);
                case 2:
                    return string.Format("{0:s}.{1:s}", owner, parts[1]);
                default:
                    return string.Format("{0:s}.{1:s}.{2:s}", parts[0], owner, parts[2]);
            }
        }

        public void ReplaceDatasourceInLyrFile(string lyrFilePath, string datasourcePath, string targetLyrFilePath)
        {
            try
            {
                FileInfo lyrFileName = new FileInfo(lyrFilePath);
                _targetWorkspace = Utils.Utils.OpenWorkspace(datasourcePath);

                //open old mxd file
                LayerFile lyfFile = new LayerFileClass();
                lyfFile.Open(lyrFileName.FullName);

                ILayer rootLayer = lyfFile.Layer;
                replaceDatasourceForLayer(rootLayer);

                #region save Lyrfile

                // overwrite ? 
                if (File.Exists(targetLyrFilePath)) File.Delete(targetLyrFilePath);

                // save changes
                lyfFile.SaveAs(targetLyrFilePath);

                // close old lyrfile
                lyfFile.Close();

                #endregion
            }


            catch (Exception ex)
            {
                _log.Error(string.Format("ReplaceDatasource {0:s}", ex.Message), ex);
            }
        }

    }
}