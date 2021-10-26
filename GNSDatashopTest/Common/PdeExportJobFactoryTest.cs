using System;
using System.Collections.Generic;
using GEOCOM.GNSD.Common.JobFactory;
using GEOCOM.GNSD.Common.Model;
using NUnit.Framework;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Test class for the PdeExportJobFactory
    /// </summary>
    [TestFixture]
    public class PdeExportJobFactoryTest
    {
        /// <summary>
        /// Tests that the CreateJob method should throw an exception with a null profile GUID.
        /// </summary>
        [Test]
        public void CreateJobShouldThrowExceptionWithNullProfileGuid()
        {
            var factory = new PdeExportJobFactory();

            Assert.Throws<ArgumentNullException>(() => factory.CreateJob(null, OutputFormat.None, new List<ExportPerimeter>()));
        }

        /// <summary>
        /// Tests that the CreateJob method should throw an exception with an empty profile GUID.
        /// </summary>
        [Test]
        public void CreateJobShouldThrowExceptionWithEmptyProfileGuid()
        {
            var factory = new PdeExportJobFactory();

             Assert.Throws<ArgumentNullException>(() => factory.CreateJob(string.Empty, OutputFormat.None, new List<ExportPerimeter>()));
        }

        /// <summary>
        /// Tests that the CreateJob method should throw an exception with an incorrect output format.
        /// </summary>
        [Test]
        public void CreateJobShouldThrowExceptionWithIncorrectOutputFormat()
        {
            var factory = new PdeExportJobFactory();

            Assert.Throws<ArgumentException>(() => factory.CreateJob(Guid.NewGuid().ToString(), OutputFormat.None, new List<ExportPerimeter>()));
        }

        /// <summary>
        /// Tests that the CreateJob method should throw an exception with null perimeters.
        /// </summary>
        [Test]
        public void CreateJobShouldThrowExceptionWithNullPerimeters()
        {
            var factory = new PdeExportJobFactory();

            Assert.Throws<ArgumentNullException>(() => factory.CreateJob(Guid.NewGuid().ToString(), OutputFormat.fgdb, null));
        }

        /// <summary>
        ///Tests that the CreateJob method should return a model.
        /// </summary>
        [Test]
        public void CreateJobShouldReturnModel()
        {
            var factory = new PdeExportJobFactory();

            var actual = factory.CreateJob(Guid.NewGuid().ToString(), OutputFormat.fgdb, new List<ExportPerimeter>());

            Assert.IsNotNull(actual);
        }
    }
}