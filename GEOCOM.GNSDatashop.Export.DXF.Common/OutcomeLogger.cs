using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class OutcomeLogger
    {
        private static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        private StoLanguage _lng;

        private bool _startLogged = false;

        public OutcomeLogger(StoLanguage languageTranslator)
        {
            _lng = languageTranslator;
        }

        #region log start/stop
        public void LogStart()
        {
            if (!_startLogged)
            {
                _startLogged = true;
                _log.Info($"{Product.Name} Ver. {Product.Version}");
            }
        }

        public void LogEnd()
        {
            if (_startLogged)
                _log.Info($"{Product.Name} Ver. {Product.Version} - end.");
            _startLogged = false;
        }

        #endregion

        #region log good/with warning outcome
        public void LogOutcome(IEnumerable<string> filesWritten, ExportedLayersInfo exportedLayers)
        {
            LogFilesWritten(filesWritten);
            LogLayersWritten(exportedLayers);
        }

        #endregion

        #region general irregularities/erroneous outcome logging
        public void LogIrregularities(Exception ex, IEnumerable<string> filesWritten, ErroneousLayersInfo layers, ExportedLayersInfo exportedLayers, params object[] supplementaryInfo)
        {
            _log.Warn(_lng.LoadStr(null != ex ? 30102 : 30100, $"Irregularities while processing:"));
            if (filesWritten.Any())
                LogFilesWritten(filesWritten);  // Do not log "nothing exported" as this may seem od when combined with i.e. an out-of-memory message
            LogErroneousLayers(layers);
            LogException(ex);
            LogSupplementaryInfo(supplementaryInfo);
            LogLayersWritten(exportedLayers);
        }
        #endregion

        #region log erroneous layers
        private void LogErroneousLayers(ErroneousLayersInfo layers)
        {
            if (layers?.Any() ?? false)
                LogErroneousLayersCore(layers);
        }

        private void LogErroneousLayersCore(ErroneousLayersInfo layers)
        {
            _log.WarnNL(1 < layers.Count
                ? _lng.LoadStr(10911, "The listed layers were not exported")
                : _lng.LoadStr(10910, "Tle listed layer was not exported"));

            _log.WarnNL(layers.ToString(true, true));
        }
        #endregion

        #region log an unbounded array of lines
        private void LogSupplementaryInfo(params object[] supplementaryInfo)
        {
            if (supplementaryInfo?.Any() ?? false)
                LogSupplementaryInfoCore(supplementaryInfo);
        }

        private void LogSupplementaryInfoCore(params object[] supplementaryInfo)
        {
            foreach (var line in supplementaryInfo)
                _log.Warn(line);
        }
        #endregion

        #region info about files written
        private void LogFilesWritten(IEnumerable<string> filesWritten)
        {
            var nFiles = filesWritten?.Count() ?? 0;
            _log.Info(0 >= nFiles
                ? _lng.LoadStr(10412, "Nothing exported - no (or no visible) layers")
                : 2 > nFiles
                    ? _lng.FmtLoadStr(10404, "DXF data exported to {0}.", filesWritten.ElementAt(0))
                    : _lng.FmtLoadStr(10407, "{0} Files exported:", FromToFile(filesWritten))
                );
        }

        private string FromToFile(IEnumerable<string> filesWritten)
            => $"{filesWritten.ElementAt(0)}...{filesWritten.Last()}";

        #endregion

        #region log layers written

        private void LogLayersWritten(ExportedLayersInfo layers)
        {
            if (0 < (layers?.Count ?? 0))
            {
                _log.Info("Exported layers:");
                LogLayers(layers.Where(l => 0 < l.FeaturesWritten).Select(l => l.ToString()));
                LogLayers(layers.Where(l => 0 >= l.FeaturesWritten).Select(l => l.ToString()));
            }
        }

        private void LogLayers(IEnumerable<string> layers)
        {
            foreach (var layer in layers)
                _log.Info(layer);
        }

        #endregion

        #region log exceptions

        private void LogException(Exception ex)
        {
            if (null != ex)
                _log.WarnNL(ex.ToString(true));
        }

        #endregion

        #region Log written layers


        #endregion
    }
}
