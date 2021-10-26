using System.Xml;
using GEOCOM.GNSD.Common.Model;
using NUnit.Framework;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Tests the JobdescriptionBaseModelClass
    /// </summary>
    [TestFixture]
    public class JobDescriptionBaseModelTest
    {
        /// <summary>
        /// Tests that the Deserialize method should return null with a null XML string.
        /// </summary>
        [Test]
        public void DeserializeShouldReturnNullWithNullXmlString()
        {
            var actual = JobDescriptionBaseModel.Deserialize(null);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests that the Deserialize method should return null with an empty XML string.
        /// </summary>
        [Test]
        public void DeserializeShouldReturnNullWithEmptyXmlString()
        {
            var actual = JobDescriptionBaseModel.Deserialize(string.Empty);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests that the ToString method should return an empty string.
        /// </summary>
        [Test]
        public void ToStringShouldReturnEmptyString()
        {
            var model = new JobDescriptionBaseModel();

            var actual = model.ToString();

            Assert.AreEqual(string.Empty, actual);
        }

        /// <summary>
        /// Tests that the ToXml method should return an Xml formatted string.
        /// </summary>
        [Test]
        public void ToXmlShouldReturnXmlString()
        {
            var model = new JobDescriptionBaseModel();

            var actual = model.ToXml();

            Assert.IsNotNullOrEmpty(actual);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(actual);

            Assert.IsTrue(xmlDoc.HasChildNodes);
        }
    }
}