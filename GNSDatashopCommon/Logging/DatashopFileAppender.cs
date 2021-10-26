using System;
using System.IO;
using GEOCOM.GNSD.Common.Config;
using log4net.Appender;

namespace GEOCOM.GNSD.Common.Logging
{
    /// <summary>
    /// This class implements a custom FileAppender used for the Datashop.
    /// It sets the path of the logfile to the path configured in the
    /// "GnsDatashopCommon.xml" file.
    /// </summary>
    public class DatashopFileAppender : FileAppender
    {
        #region Properties

        /// <summary>
        /// Gets or sets the log file name.
        /// </summary>
        /// <value>The log file name.</value>
        public override string File
        {
            get { return base.File; }
            set
            {
                try
                {
                    var logDirectory = Path.GetDirectoryName(value);

                    if (string.IsNullOrEmpty(logDirectory))
                        logDirectory = GnsDatashopCommonConfig.LogDirectoryPath;

                    base.File = Path.Combine(logDirectory, Path.GetFileName(value));
                }
                catch (Exception)
                {
                    // use the default
                    base.File = value;
                }
            }
        }

        #endregion
    }
}