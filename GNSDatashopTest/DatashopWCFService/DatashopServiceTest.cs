using GEOCOM.GNSD.Web.Core.Service;
using NUnit.Framework;

namespace GNSDatashopTest.DatashopWCFService
{
    [TestFixture]
    public class DatashopServiceTest
    {
        [Test]
        public void ShouldCreateAdminChannel()
        {
            var adminClient = DatashopService.Instance.AdminService;

            Assert.IsNotNull(adminClient);
        }

        [Test]
        public void ShouldCreateJobManagerChannel()
        {
            var jobClient = DatashopService.Instance.JobService;

            Assert.IsNotNull(jobClient);
        }

        [Test]
        public void ShouldCreateDocumentChannel()
        {
            var documentClient = DatashopService.Instance.DocumentService;

            Assert.IsNotNull(documentClient);
        }

        [Test]
        public void ShouldCreateAddressChannel()
        {
            var addressClient = DatashopService.Instance.AddressSearchService;

            Assert.IsNotNull(addressClient);
        }
    }
}