using System;
using System.IO;
using System.Reflection.Emit;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSD.DatashopWorkflow.GeoAttachments;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace GNSDatashopTest.DatashopWorkflow.GeoAttachmentsTests
{
    [TestFixture]
    public class GeoAttachmentCollectionTest
    {
        const long BytesInMegaByte = 1048576; // 1048576 = 1014 * 1024 is conversion from bytes to megabytes.

        #region Test methods

        [Test]
        public void EmptyCollection()
        {
            var attachmentCollection = new GeoAttachmentCollection();
            attachmentCollection.CheckTotalSize("0");
        }

        [Test]
        public void CheckTotalSizeWithNonNumbericalValue()
        {
            var attachmentCollection = new GeoAttachmentCollection();
            attachmentCollection.CheckTotalSize("this is not a number");
        }

        [Test]
        public void AddOneFeature()
        {
            // feature mock
            var featureId = 123;
            var fieldIndex = 666;
            var filePathFieldName = "fieldName";
            string filePath = "x:\\filepath.txt";
            var fileSize = 10;

            var feature = MockFeature(featureId, fieldIndex, filePath);
            var geoAttachment = MockAttachment(featureId, filePath, fileSize);

            // tested class + mocking some methods
            var attachmentCollection = Mock.Of<GeoAttachmentCollection>(
                collection =>
                collection.FindFieldIndex(feature, filePathFieldName) == fieldIndex &&
                collection.CreateGeoAttachment(featureId, filePath) == geoAttachment
                );

            // actual test
            attachmentCollection.AddFromFeature(feature, filePathFieldName);

            var attachment = attachmentCollection.First();
            Assert.AreEqual(filePath, attachment.FilePath);
            Assert.AreEqual(featureId, attachment.FeatureId);
            Assert.AreEqual(fileSize, attachment.FileSize);

            // should not throw an exception since 10 B < 10 MB
            attachmentCollection.CheckTotalSize("10");
        }

        [Test]
        public void AddTwoUniqueFeatures()
        {
            int fieldIndex = 666;
            var filePathFieldName = "fieldName";

            // first feature and geo-attachment 
            int featureId01 = 1;
            string filePath01 = "x:\\path01";
            var feature01 = MockFeature(featureId01, fieldIndex, filePath01);
            var geoAttachment01 = MockAttachment(featureId01, filePath01, 1 * BytesInMegaByte);

            // second feature and geo-attachment 
            int featureId02 = 2;
            string filePath02 = "x:\\path02";
            var feature02 = MockFeature(featureId02, fieldIndex, filePath02);
            var geoAttachment02 = MockAttachment(featureId02, filePath02, 2 * BytesInMegaByte);

            // tested class + mocking some methods
            var attachmentCollection = Mock.Of<GeoAttachmentCollection>(
                collection =>
                collection.FindFieldIndex(It.IsAny<IFeature>(), filePathFieldName) == fieldIndex &&
                collection.CreateGeoAttachment(featureId01, filePath01) == geoAttachment01 &&
                collection.CreateGeoAttachment(featureId02, filePath02) == geoAttachment02);

            // actual test
            attachmentCollection.AddFromFeature(feature01, filePathFieldName);
            attachmentCollection.AddFromFeature(feature02, filePathFieldName);
            var attachment01 = attachmentCollection.First();
            var attachment02 = attachmentCollection.ElementAt(1);

            // assert 
            Assert.IsNotNull(attachment01);
            Assert.IsNotNull(attachment02);

            // should not throw an exception
            attachmentCollection.CheckTotalSize("3");
        }

        [Test]
        public void AddFromFeatures()
        {
            int fieldIndex = 666;
            var filePathFieldName = "fieldName";

            // first feature and geo-attachment 
            int featureId01 = 1;
            string filePath01 = "x:\\path01";
            var feature01 = MockFeature(featureId01, fieldIndex, filePath01);
            var geoAttachment01 = MockAttachment(featureId01, filePath01, 1 * BytesInMegaByte);

            // second feature and geo-attachment 
            int featureId02 = 2;
            string filePath02 = "x:\\path02";
            var feature02 = MockFeature(featureId02, fieldIndex, filePath02);
            var geoAttachment02 = MockAttachment(featureId02, filePath02, 2 * BytesInMegaByte);

            // tested class + mocking some methods
            var attachmentCollection = Mock.Of<GeoAttachmentCollection>(
                  collection =>
                  collection.FindFieldIndex(It.IsAny<IFeature>(), filePathFieldName) == fieldIndex &&
                  collection.CreateGeoAttachment(featureId01, filePath01) == geoAttachment01 &&
                  collection.CreateGeoAttachment(featureId02, filePath02) == geoAttachment02);

            // actual test
           attachmentCollection.AddFromFeatures(new[] { feature01, feature02 }, filePathFieldName);

           Assert.IsNotNull(attachmentCollection.ElementAt(0));
           Assert.IsNotNull(attachmentCollection.ElementAt(1));
        }

        [Test]
        public void CopyAllFeatures()
        {
            int fieldIndex = 666;
            var filePathFieldName = "fieldName";

            // first feature and geo-attachment 
            int featureId01 = 1;
            string filePath01 = "x:\\path01.txt";
            var feature01 = MockFeature(featureId01, fieldIndex, filePath01);
            var geoAttachment01 = MockAttachment(featureId01, filePath01, 1 * BytesInMegaByte);

            // second feature and geo-attachment 
            int featureId02 = 2;
            string filePath02 = "x:\\path02.txt";
            var feature02 = MockFeature(featureId02, fieldIndex, filePath02);
            var geoAttachment02 = MockAttachment(featureId02, filePath02, 2 * BytesInMegaByte);

            // tested class + mocking some methods
            var attachmentCollection = Mock.Of<GeoAttachmentCollection>(
                collection =>
                collection.FindFieldIndex(It.IsAny<IFeature>(), filePathFieldName) == fieldIndex &&
                collection.CreateGeoAttachment(featureId01, filePath01) == geoAttachment01 &&
                collection.CreateGeoAttachment(featureId02, filePath02) == geoAttachment02);

            // actual test
            attachmentCollection.AddFromFeatures(new[] { feature01, feature02 }, filePathFieldName);
            var destinationFolder = "z:\\destination";
            var targetPaths = attachmentCollection.CopyAll(destinationFolder);

            // asserts
            Assert.IsNotNull(targetPaths);
            var targetPath01 = "z:\\destination\\path01.txt";
            Assert.IsTrue(targetPaths.Contains(targetPath01));
            var targetPath02 = "z:\\destination\\path02.txt";
            Assert.IsTrue(targetPaths.Contains(targetPath02));

            var collectionMock = Mock.Get(attachmentCollection);
            // call verifications
            collectionMock.Verify(collection => collection.CreateFolderIfNeeded(destinationFolder), Times.Once());
            collectionMock.Verify(collection => collection.CopyFile(filePath01, targetPath01, featureId01), Times.Once());
            collectionMock.Verify(collection => collection.CopyFile(filePath02, targetPath02, featureId02), Times.Once());
        }

        [Test]
        public void CopyAllFeaturesEmptyCollection()
        {
            // tested class + mocking some methods
            var attachmentCollection = Mock.Of<GeoAttachmentCollection>();

            // actual test
            var paths = attachmentCollection.CopyAll("z:\\destination");
            Assert.AreEqual(0, paths.Count);

            var collectionMock = Mock.Get(attachmentCollection);
            // call verifications
            collectionMock.Verify(collection => collection.CopyFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            collectionMock.Verify(collection => collection.CreateFolderIfNeeded(It.IsAny<string>()), Times.Never());
        }

        //[Test]
        //public void AddThreeNotUniqueFeatures()
        //{
        //    int fieldIndex = 666;
        //    var filePathFieldName = "fieldName";

        //    // first feature and geo-attachment 
        //    int featureId01 = 1;
        //    string filePath01 = "x:\\path01";
        //    var feature01 = MockFeature(featureId01, fieldIndex, filePath01);
        //    var geoAttachment01 = MockAttachment(featureId01, filePath01, 1 * BytesInMegaByte);

        //    // second feature and geo-attachment 
        //    int featureId02 = 2;
        //    string filePath02 = "x:\\path02";
        //    var feature02 = MockFeature(featureId02, fieldIndex, filePath02);
        //    var geoAttachment02 = MockAttachment(featureId02, filePath02, 2 * BytesInMegaByte);

        //    // tested classes + mocking some methods
        //    var attachmentCollection = Mock.Of<GeoAttachmentCollection>(
        //        collection =>
        //        collection.FindFieldIndex(It.IsAny<IFeature>(), filePathFieldName) == fieldIndex &&
        //        collection.CreateGeoAttachment(featureId01, filePath01) == geoAttachment01 &&
        //        collection.CreateGeoAttachment(featureId02, filePath02) == geoAttachment02);

        //    attachmentCollection.AddFromFeature(feature01, "fieldName");
        //    attachmentCollection.AddFromFeature(feature02, "fieldName");
        //    attachmentCollection.AddFromFeature(feature02, "fieldName");

        //    int maxSizeMB = 3;
        //    var ex01 = Assert.Throws<GeoAttachmentsMaxSizeExceededException>(() => attachmentCollection.CheckTotalSize(maxSizeMB.ToString()));
        //    Assert.AreEqual(3 * BytesInMegaByte, ex01.MaxSize);
        //    Assert.AreEqual((1 + 2 + 2) * BytesInMegaByte, ex01.ActualSize);

        //    Assert.Throws<ApplicationException>(() => attachmentCollection.EnsureUniquePathes());
        //}

        #endregion

        #region Helpers

        private static IGeoAttachment MockAttachment(int featureId, string filePath, long fileSize)
        {
            var attachment = Mock.Of<IGeoAttachment>(
                attch =>
                attch.FeatureId == featureId &&
                attch.FilePath == filePath &&
                attch.FileSize == fileSize);
            return attachment;
        }

        private static IFeature MockFeature(int featureId, int fieldIndex, string filePath)
        {
            var feature = Mock.Of<IFeature>(
                ftr =>
                ftr.OID == featureId &&
#pragma warning disable 252,253
                ftr.get_Value(fieldIndex) == filePath);
#pragma warning restore 252,253
            return feature;
        }

        #endregion
    }
}