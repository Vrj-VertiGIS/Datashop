using System;
using System.ServiceModel;
using GEOCOM.GNSDatashop.ServiceClient;
using Moq;
using NUnit.Framework;

namespace GNSDatashopTest.ServiceClient
{
    /// <summary>
    /// Unit tests for the configurable service client
    /// </summary>
    [TestFixture]
    public class ConfigurableServiceClientTest
    {
        #region Constructor Tests

        [Test]
        public void ConstructorShouldCreateChannelFromConfig()
        {
            var client = new ConfigurableServiceClient<IServiceContract>("ClientTest");

            Assert.IsNotNull(client.Channel);
        }

        [Test]
        public void ConstructorShouldCreateChannelFromConfigWithCredentials()
        {
            var client = new ConfigurableServiceClient<IServiceContract>("ClientTest", "123", "456");

            Assert.IsNotNull(client.Channel);
        }

        [Test]
        public void ConstructorShouldThrowExceptionWithInvalidConfiguration()
        {
            var ex = Assert.Throws<Exception>(() => new ConfigurableServiceClient<IServiceContract>("BadConfig"));

            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf<InvalidOperationException>(ex.InnerException);
        }

        [Test]
        public void ConstructorShouldThrowExceptionWithInvalidConfigurationAndCredentials()
        {
            var ex = Assert.Throws<Exception>(() => new ConfigurableServiceClient<IServiceContract>("BadConfig", "123", "456"));

            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf<InvalidOperationException>(ex.InnerException);
        }

        [Test]
        public void ConstructorShouldThrowExceptionWithInvalidContract()
        {
            var ex = Assert.Throws<Exception>(() => new ConfigurableServiceClient<IServiceContract>("InvalidContractClientTest"));

            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf<InvalidOperationException>(ex.InnerException);
        }

        [Test]
        public void ConstructorShouldThrowExceptionWithInvalidContractAndCredentials()
        {
            var ex = Assert.Throws<Exception>(() => new ConfigurableServiceClient<IServiceContract>("InvalidContractClientTest", "123", "456"));

            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf<InvalidOperationException>(ex.InnerException);
        }

        //[Test]
        //public void ConstructorShouldAcceptChannel()
        //{
        //    var mockChannel = new Mock<IServiceContract>();
        //    var channel = mockChannel.Object;

        //    var client = new ConfigurableServiceClient<IServiceContract>(channel);

        //    Assert.IsNotNull(client.Channel);
        //}

        [Test]
        public void ConstructorShouldThrowExceptionWithNullConfigurationName()
        {
            string configurationName = null;

            Assert.Throws<GEOCOM.Common.Exceptions.AssertionException>(() => new ConfigurableServiceClient<IServiceContract>(configurationName));
        }

        [Test]
        public void ConstructorShouldThrowExceptionWithNullUsername()
        {
            string username = null;

            Assert.Throws<GEOCOM.Common.Exceptions.AssertionException>(() => new ConfigurableServiceClient<IServiceContract>("123", username, null));
        }

        [Test]
        public void ConstructorShouldThrowExceptionWithNullPassword()
        {
            string password = null;

            Assert.Throws<GEOCOM.Common.Exceptions.AssertionException>(() => new ConfigurableServiceClient<IServiceContract>("123", "123", password));
        }

        //[Test]
        //public void ConstructorShouldThrowExceptionWithNullChannel()
        //{
        //    var mockChannel = new Mock<IServiceContract>();
        //    var channel = mockChannel.Object;
        //    channel = null;

        //    Assert.Throws<GEOCOM.Common.Exceptions.AssertionException>(() => new ConfigurableServiceClient<IServiceContract>(channel));
        //} 

        #endregion

        #region Dispose Tests

        [Test]
        public void DisposeShouldCorrectlyCloseCommunicationObject()
        {
            var client = new ConfigurableServiceClient<IServiceContract>("ClientTest");

            Assert.IsNotNull(client.Channel);

            Assert.DoesNotThrow(client.Dispose);
        }

        [Test]
        public void DisposeShouldCorrectlyCloseCommunicationObjectWithCredentials()
        {
            var client = new ConfigurableServiceClient<IServiceContract>("ClientTest", "123", "456");

            Assert.IsNotNull(client.Channel);

            Assert.DoesNotThrow(client.Dispose);
        }

        #endregion
    }

    [ServiceContract]
    public interface IServiceContract
    {
        [OperationContract]
        void ServiceOperation();
    }

    public class ServiceContractImplementation : IServiceContract
    {
        public void ServiceOperation()
        {
            
        }
    }
}