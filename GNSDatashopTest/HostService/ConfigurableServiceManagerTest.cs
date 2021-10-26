using System;
using System.Configuration;
using GEOCOM.GNSDatashop.HostService;
using NUnit.Framework;

namespace GNSDatashopTest.HostService
{
    [TestFixture]
    public class ConfigurableServiceManagerTest
    {
        #region Constructor Tests

        [Test]
        public void ConstructorShouldThrowExceptionWithNullConfig()
        {
            Configuration config = null;

            Assert.Throws<GEOCOM.Common.Exceptions.AssertionException>(() => new ConfigurableServiceManager(config));
        }

        #endregion

        #region Initialise and Dispose Tests
        //TODO Make test pass
        // [Test]
        public void ShouldInitialiseAndDisposeServicesFromStandardConfiguration()
        {
            var serviceManager = new ConfigurableServiceManager();

            Assert.DoesNotThrow(serviceManager.InitializeServices);

            Assert.DoesNotThrow(serviceManager.Dispose);
        }
        //TODO Make test pass
        //[Test]
        public void ShouldInitialiseAndDisposeServicesFromExternalConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var serviceManager = new ConfigurableServiceManager(config);

            Assert.DoesNotThrow(serviceManager.InitializeServices);

            Assert.DoesNotThrow(serviceManager.Dispose);
        }

        #endregion
    }
}
