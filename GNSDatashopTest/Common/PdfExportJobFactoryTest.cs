using System;
using System.Collections.Generic;
using GEOCOM.GNSD.Common.JobFactory;
using GEOCOM.GNSD.Common.Model;
using NUnit.Framework;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Test class for PdfExportJobFactory
    /// </summary>
    [TestFixture]
    public class PdfExportJobFactoryTest
    {
        /// <summary>
        /// Tests the CreateJob method to throw an exception with null map extents.
        /// </summary>
        [Test]
        [ExpectedException(ExpectedException = typeof(Exception), ExpectedMessage="No map extends defined!")]
        public void CreateJobShouldThrowExceptionWithNullMapExtents()
        {
            var factory = new PdfExportJobFactory();

            factory.CreateJob(null);
        }

        /// <summary>
        /// Tests the CreateJob method to return a LayoutExportModel even with a zero length collection of extents
        /// </summary>
        [Test]
        public void CreateJobShouldReturnModel()
        {
            var mapExtents = new List<MapExtent>();

            var factory = new PdfExportJobFactory();

            var expected = factory.CreateJob(mapExtents);

            Assert.IsNotNull(expected);
        }
    }
}