using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using log4net.Appender;
using log4net.Core;

namespace GEOCOM.GNSD.PlotExtension.Log
{
    /// <summary>
    /// ArcGIS Server Logging Appender.
    /// Forwards loggingEvents generated with the log4Net Api
    /// to the ESRI own Server-Log.
    /// </summary>
    public class AgsLogAppender : AppenderSkeleton
    {
        #region Constants

        /// <summary>
        /// ArcGIS Server LogLevel [None] Code
        /// </summary>
        private const int AgsLogLevelNone = 0;

        /// <summary>
        /// ArcGIS Server LogLevel [Error] Code
        /// </summary>
        private const int AgsLogLevelError = 1;

        /// <summary>
        /// ArcGIS Server LogLevel [Warn] Code
        /// </summary>
        private const int AgsLogLevelWarn = 2;

        /// <summary>
        /// ArcGIS Server LogLevel [Info] Code
        /// </summary>
        private const int AgsLogLevelInfo = 4;

        /// <summary>
        /// ArcGIS Server LogLevel [Debug] Code
        /// </summary>
        private const int AgsLogLevelDebug = 6;

        /// <summary>
        /// ArcGIS Server Message- Code
        /// </summary>
        private const int AgsLogMessageCode = 8000;

        #endregion // Constants

        /// <summary>
        /// Gets or sets the ArcGIS Server logger instace.
        /// </summary>
        /// <value>The ArcGIS Server log instance.</value>
        public ILog2 AgsLog { get; set; }

        #region Overrides of AppenderSkeleton

        /// <summary>
        /// Appends the specified logging event to the ArcGIS Server Log.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.Level == Level.Debug)
            {
                Log(loggingEvent, AgsLogLevelDebug);
            }
            else if (loggingEvent.Level == Level.Info)
            {
                Log(loggingEvent, AgsLogLevelInfo);
            }
            else if (loggingEvent.Level == Level.Warn)
            {
                Log(loggingEvent, AgsLogLevelWarn);
            }
            else if (loggingEvent.Level == Level.Fatal ||
                     loggingEvent.Level == Level.Error)
            {
                Log(loggingEvent, AgsLogLevelError);
            } 
            else
            {
                Log(loggingEvent, AgsLogLevelNone);    
            }
        }

        #endregion // Overrides of AppenderSkeleton

        /// <summary>
        /// Logs the specified loggin event to the ArcGIS Server log.
        /// </summary>
        /// <param name="logginEvent">The loggin event.</param>
        /// <param name="agsLogLevel">The log level.</param>
        private void Log(LoggingEvent logginEvent, int agsLogLevel)
        {
            if (AgsLog != null && AgsLog.WillLog(agsLogLevel))
            {
                string asm = "GNSDatashopJobExecutorExtension";    // TODO: Get using reflection
                AgsLog.AddMessage(agsLogLevel, AgsLogMessageCode, asm + ": " + logginEvent.RenderedMessage);
            }
        }
    }
}
