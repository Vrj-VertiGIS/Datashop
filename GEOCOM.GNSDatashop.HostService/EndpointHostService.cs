using System;
using System.ServiceProcess;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;

namespace GEOCOM.GNSDatashop.HostService
{
    /// <summary>
    /// Class that hosts the WCF EndPoint implementations
    /// </summary>
    partial class EndpointHostService : ServiceBase
    {
        #region Private members

        /// <summary>
        /// Holds the instance of the ConfigurableServiceManager class that starts and stops the endpoints
        /// </summary>
        private readonly ConfigurableServiceManager serviceManager;

        /// <summary>
        /// Holds the instance of the logger
        /// </summary>
        private readonly IMsg logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointHostService"/> class.
        /// </summary>
        public EndpointHostService()
        {
            InitializeComponent();

            DatashopLogInitializer.Initialize();

            this.serviceManager = new ConfigurableServiceManager();

            this.logger = new Msg(typeof (EndpointHostService));
        } 

        #endregion

        #region Base Class Overrides

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            this.logger.Debug("EndpointHostService starting");

            try
            {
                this.logger.Debug("Initialising Service Endpoints");

                this.serviceManager.InitializeServices();

                foreach (var s in this.serviceManager.HostedServiceNames)
                    this.logger.DebugFormat("Started service: {0}", s);

                this.logger.Debug("Service Endpoints Initialised");
            }
            catch (Exception ex)
            {
                var msg = string.Format("Fatal error during startup of EndpointHostService with arguments: {0}",
                                        string.Join(", ", args));

                this.logger.Fatal(msg, ex);

                throw new Exception(msg, ex);
            }

            this.logger.Debug("EndpointHostService started successfully");
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            this.logger.Debug("EndpointHostService shutting down");

            try
            {
                this.logger.Debug("Disposing Service Endpoints");

                this.serviceManager.Dispose();

                this.logger.Debug("Service Endpoints disposed successfully");
            }
            catch (Exception ex)
            {
                const string msg = "Fatal error during shutdown of EndpointHostService";

                this.logger.Fatal(msg, ex);

                throw new Exception(msg, ex);
            }

            this.logger.Debug("EndpointHostService shut down successfully");
        } 

        #endregion
    }
}