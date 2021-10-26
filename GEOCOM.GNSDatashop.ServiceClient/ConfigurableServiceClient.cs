using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Description;
using GEOCOM.Common;

namespace GEOCOM.GNSDatashop.ServiceClient
{
    /// <summary>
    /// Wraps the channelfactory
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    public class ConfigurableServiceClient<TChannel> : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets the channel. This property holds the concrete implementation of the service contract.
        /// </summary>
        public TChannel Channel 
        {
            get { return this.channelFactory.CreateChannel(); }
        } 

        #endregion

        #region Private members

        /// <summary>
        /// Holds the channelfactory for this instance
        /// </summary>
        private readonly ChannelFactory<TChannel> channelFactory; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableServiceClient&lt;TChannel&gt;"/> class.
        /// </summary>
        /// <param name="configurationName">Name of the configuration.</param>
        public ConfigurableServiceClient(string configurationName)
        {
            Assert.True(!string.IsNullOrWhiteSpace(configurationName), "configurationName cannot be null, empty or whitespace");

            try
            {
                this.channelFactory = new ChannelFactory<TChannel>(configurationName);

                //this.Channel = this.channelFactory.CreateChannel();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading endpoint configuration: {0}", configurationName), ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableServiceClient&lt;TChannel&gt;"/> class.
        /// </summary>
        /// <param name="configurationName">Name of the configuration.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public ConfigurableServiceClient(string configurationName, string userName, string password)
        {
            Assert.True(!string.IsNullOrWhiteSpace(configurationName), "configurationName cannot be null, empty or whitespace");
            Assert.True(!string.IsNullOrWhiteSpace(userName), "userName cannot be null, empty or whitespace");
            Assert.True(!string.IsNullOrWhiteSpace(password), "password cannot be null, empty or whitespace");

            try
            {
                this.channelFactory = new ChannelFactory<TChannel>(configurationName);

                this.ApplyCredentials(userName, password);

                //this.Channel = this.channelFactory.CreateChannel();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading endpoint configuration: {0} for username: {1}", configurationName, userName), ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the credentials.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        private void ApplyCredentials(string userName, string password)
        {
            var defaultCredentials = this.channelFactory.Endpoint.Behaviors.Find<ClientCredentials>();

            defaultCredentials.UserName.UserName = userName;
            defaultCredentials.UserName.Password = password;
        }

        /// <summary>
        /// Disposes the communication object.
        /// </summary>
        /// <param name="communicationObject">The communication object.</param>
        private void DisposeCommunicationObject(ICommunicationObject communicationObject)
        {
            if (communicationObject != null)
            {
                try
                {
                    communicationObject.Close();
                }
                catch (CommunicationException)
                {
                    communicationObject.Abort();
                }
                catch (TimeoutException)
                {
                    communicationObject.Abort();
                }
                catch (Exception ex)
                {
                    communicationObject.Abort();

                    throw new Exception("Unhandled error during disposal of CommunicationObject", ex);
                }
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.DisposeCommunicationObject(this.channelFactory);

            //this.DisposeCommunicationObject(this.Channel as ICommunicationObject);
        }

        #endregion
    }
}