using System;
using GEOCOM.GNSDatashop.ServiceClient;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSD.Web.Core.Service
{
    /// <summary>
    /// Singleton that wraps the ConfigurableServiceClient instances into a singleton for easy one line access
    /// </summary>
    public sealed class DatashopService
    {
        #region Private Members

        /// <summary>
        /// Holds the single instance
        /// </summary>
        private static volatile DatashopService instance;

        /// <summary>
        /// Provides thread safe object locking
        /// </summary>
        private static readonly object syncRoot = new Object();

        /// <summary>
        /// Holds the client for the admin service
        /// </summary>
        private readonly ConfigurableServiceClient<IAdminDatashop> adminClient;

        /// <summary>
        /// holds the client for the job manager service
        /// </summary>
        private readonly ConfigurableServiceClient<IJobManager> jobManagerClient;

        /// <summary>
        /// Holds the client for the document service
        /// </summary>
        private readonly ConfigurableServiceClient<IDocumentService> documentClient;

        /// <summary>
        /// Holds the client for the address search service
        /// </summary>
        private readonly ConfigurableServiceClient<IAddressSearchService> addressSearchClient;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the admin service.
        /// </summary>
        public IAdminDatashop AdminService
        {
            get { return this.adminClient.Channel; }
        }

        /// <summary>
        /// Gets the job service.
        /// </summary>
        public IJobManager JobService
        {
            get { return this.jobManagerClient.Channel; }
        }

        /// <summary>
        /// Gets the document service.
        /// </summary>
        public IDocumentService DocumentService
        {
            get { return this.documentClient.Channel; }
        }

        /// <summary>
        /// Gets the address search service.
        /// </summary>
        public IAddressSearchService AddressSearchService
        {
            get { return this.addressSearchClient.Channel; }
        }

        /// <summary>
        /// Public getter for the single instance
        /// </summary>
        public static DatashopService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DatashopService();
                    }
                }

                return instance;
            }
        } 

        #endregion

        #region Constructor / Destructor

        /// <summary>
        /// Prevents a default instance of the <see cref="DatashopService"/> class from being created.
        /// </summary>
        private DatashopService()
        {
            try
            {
                this.adminClient = new ConfigurableServiceClient<IAdminDatashop>("AdminService");

                this.jobManagerClient = new ConfigurableServiceClient<IJobManager>("JobService");

                this.documentClient = new ConfigurableServiceClient<IDocumentService>("DocumentService");

                this.addressSearchClient = new ConfigurableServiceClient<IAddressSearchService>("AddressSearchService");
            }
            catch (Exception ex)
            {
                throw new Exception("Fatal error initializing DatashopService", ex);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DatashopService"/> is reclaimed by garbage collection.
        /// </summary>
        ~DatashopService()
        {
			DisposeSafely(this.adminClient);
            DisposeSafely(this.jobManagerClient);
            DisposeSafely(this.documentClient);
            DisposeSafely(this.addressSearchClient);
        }

		private void DisposeSafely(IDisposable objectToDispose)
		{
			if(objectToDispose != null)
				objectToDispose.Dispose();
		}

        #endregion
    }
}