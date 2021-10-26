using System;
using GEOCOM.GNSD.Common.Model;
using NUnit.Framework;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Tests the ExportModel class
    /// </summary>
    [TestFixture]
    public class TdeExportModelTest
    {
        /// <summary>
        /// Tests that the ToString method should return a correctly formatted string.
        /// </summary>
        [Test]
        public void ToStringShouldReturnCorrectlyFormattedString()
        {
            var profileGuid = Guid.NewGuid().ToString();

            var model = new TdeExportModel { ProfileGuid = profileGuid, Perimeters = new ExportPerimeter[0] };

            var actual = model.ToString();

            Assert.IsNotNullOrEmpty(actual);
            Assert.AreEqual(string.Format("   ProfileGuid: {0}", profileGuid), actual);
        }
    }
}