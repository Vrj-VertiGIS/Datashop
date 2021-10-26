using GEOCOM.GNSD.Common.Model;
using NUnit.Framework;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Tests the AdminJobModel class
    /// </summary>
    [TestFixture]
    public class AdminJobModelTest
    {
        /// <summary>
        /// Tests that the ToString method should return a correctly formatted string.
        /// </summary>
        [Test]
        public void ToStringShouldReturnCorrectlyFormattedString()
        {
            var model = new AdminJobModel { Action = "Test" };

            var actual = model.ToString();

            Assert.IsNotNullOrEmpty(actual);
            Assert.AreEqual(" Action=Test", actual);
        }
    }
}