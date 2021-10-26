using System;
using System.IO;
using GEOCOM.GNSD.DatashopWorkflow.GeoAttachments;
using NUnit.Framework;

namespace GNSDatashopTest.DatashopWorkflow.GeoAttachmentsTests
{
    [TestFixture]
    public class GeoAttachmentTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GeoAttachmentPathEmpty()
        {
            new GeoAttachment(1, string.Empty, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GeoAttachmentPathNull()
        {
            new GeoAttachment(1, null, 1);
        }

        [Test]
        public void GeoAttachment()
        {
            var sizeBytes = 3;
            var tempFileName = "c:\\somefile.txt";

            // test and assert
            var featureId = 1;
            var geoAttachment = new GeoAttachment(featureId, tempFileName, sizeBytes);
            Assert.AreEqual(featureId, geoAttachment.FeatureId);
            Assert.AreEqual(tempFileName, geoAttachment.FilePath);
            Assert.AreEqual(sizeBytes, geoAttachment.FileSize);
        }

        private static string CreateTempFile(long sizeBytes)
        {
            // create a temp file
            var tempFileName = Path.GetTempFileName();
            var fileStream = File.OpenWrite(tempFileName);
            for (int i = 0; i < sizeBytes; i++)
            {
                fileStream.WriteByte((byte)i);
            }
            fileStream.Close();

            return tempFileName;
        }
    }
}
