using NUnit.Framework;
using GEOCOM.GNSD.Common.Model;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Tests the ExportModel class
    /// </summary>
    [TestFixture]
    public class ExportModelTest
    {
        /// <summary>
        /// Tests that the ToString method should return a non empty string with a zero length perimeter collection
        /// </summary>
        [Test]
        public void ToStringShouldReturnNonEmptyStringWithZeroLengthPerimeters()
        {
            var model = new ExportModel { Perimeters = new ExportPerimeter[0] };

            var actual = model.ToString();

            Assert.IsNotNullOrEmpty(actual);
        }
    }
}