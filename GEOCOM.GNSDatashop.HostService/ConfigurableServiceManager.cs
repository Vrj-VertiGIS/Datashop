using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using GEOCOM.Common;

namespace GEOCOM.GNSDatashop.HostService
{
    /// <summary>
    /// Class that handles the initialization and disposal of WCF ServiceHosts from configuration
    /// </summary>
    public class ConfigurableServiceManager : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets the hosted service names.
        /// </summary>
        public IEnumerable<string> HostedServiceNames 
        { 
            get { return this.serviceHosts.ConvertAll(s => s.Description.Name); }
        }

        #endregion

        #region Private members

        /// <summary>
        /// Holds the configuration for this instance
        /// </summary>
        private readonly Configuration config;

        /// <summary>
        /// Holds all the created servicehosts for this instance
        /// </summary>
        private readonly List<ServiceHost> serviceHosts = new List<ServiceHost>();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableServiceManager"/> class.
        /// </summary>
        public ConfigurableServiceManager()
            : this(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableServiceManager"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public ConfigurableServiceManager(Configuration config)
        {
            Assert.True(config != null, "config cannot be null");

            this.config = config;
        } 

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes the services by looping through the System.ServiceModel config
        /// </summary>
        public void InitializeServices()
        {
            try
            {
                var services = ServiceModelSectionGroup.GetSectionGroup(this.config);

                if (services != null)
                    services.Services.Services.Cast<ServiceElement>()
                        .ToList()
                        .ForEach(s => this.StartServiceHost(s.Name));
            }
            catch (Exception ex)
            {
                throw new Exception("Fatal error during InitializeServices()", ex);
            }
        }

        /// <summary>
        /// Starts the service host.
        /// </summary>
        /// <param name="serviceTypeName">Name of the service type.</param>
        public void StartServiceHost(string serviceTypeName)
        {
            Assert.True(serviceTypeName != null, "serviceTypeName");

            //TODO: fix assembly resolver issue
            var serviceType = Type.GetType(string.Format("{0}, GEOCOM.GNSDatashop.Services", serviceTypeName));

            if (serviceType != null)
            {
                try
                {
                    var host = new ServiceHost(serviceType);
                    host.AddDefaultEndpoints();
                    host.Open();

                    this.serviceHosts.Add(host);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error starting service host: {0}", serviceTypeName), ex);
                }
            }
            else
                throw new Exception(string.Format("Could not resolve type for service host: {0}", serviceTypeName));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Disposes the service host cleanly:
        /// http://www.danrigsby.com/blog/index.php/2008/02/26/dont-wrap-wcf-service-hosts-or-clients-in-a-using-statement/
        /// </summary>
        /// <param name="serviceHost">The service host.</param>
        private void DisposeServiceHost(ServiceHost serviceHost)
        {
            if (serviceHost != null)
            {
                try
                {
                    serviceHost.Close();
                }
                catch (CommunicationException)
                {
                    serviceHost.Abort();
                }
                catch (TimeoutException)
                {
                    serviceHost.Abort();
                }
                catch (Exception ex)
                {
                    serviceHost.Abort();

                    throw new Exception(string.Format("Unhandled error during disposal of ServiceHost {0}", serviceHost.Description), ex);
                }
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.serviceHosts != null)
                this.serviceHosts.ForEach(this.DisposeServiceHost);
        } 

        #endregion
    }
}