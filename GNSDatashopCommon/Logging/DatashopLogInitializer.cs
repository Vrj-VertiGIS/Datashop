namespace GEOCOM.GNSD.Common.Logging
{
    /// <summary>
    /// Class that handles the initialization of the Datashop Logger
    /// </summary>
    public static class DatashopLogInitializer
    {
        /// <summary>
        /// Flag that indicates if the log has been initialized
        /// </summary>
        private static bool isConfigured = false;

        /// <summary>
        /// Initializes the logger.
        /// - Calls log4net.Config.XmlConfigurator.Configure()
        /// This method should be called once for each Datashop application!
        /// </summary>
        public static void Initialize()        
        {
            if (!isConfigured)
            {
                // use config from web.config / app.config
                log4net.Config.XmlConfigurator.Configure();

                isConfigured = true;
            }
        }

        /// <summary>
        /// Initializes the specified file param.
        /// </summary>
        /// <param name="fileParam">The file param.</param>
        public static void Initialize(string fileParam)
        {
            if (!isConfigured)
            {
                // use config from web.config / app.config
                log4net.GlobalContext.Properties["gnsd_jobid"] = fileParam;
                log4net.Config.XmlConfigurator.Configure();

                isConfigured = true;
            }            
        }
    }
}