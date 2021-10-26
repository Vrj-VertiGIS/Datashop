using GEOCOM.GNSD.Common.Config;
using GNSDatashopTest.TestUtils;
using NUnit.Framework;

namespace GNSDatashopTest.Common
{
    /// <summary>
    /// Tests the AuthenticationInfo class
    /// </summary>
    [TestFixture]
    public class AuthenticationInfoTest
    {
        /// <summary>
        /// Holds the instance of the class under test
        /// </summary>
        private AuthenticationInfo authenticationInfo;

        /// <summary>
        /// plain text string
        /// </summary>
        private string toEncrypt = "StringToEncrypt";

        /// <summary>
        /// encrypted string of the plain text string
        /// </summary>
        private string toDecrypt = "K0oODJGcWYTzSg+ZZBot+Q==";

        /// <summary>
        /// Sets up this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.authenticationInfo = new AuthenticationInfo();
        }

        /// <summary>
        /// Tests that the Encrypt should return an encrypted string.
        /// </summary>
        [Test]
        public void EncryptShouldReturnEncryptedString()
        {
            var encryptMethod = this.authenticationInfo.GetType()
                .GetInstanceMethod("Encrypt");

            var actual = encryptMethod.Invoke(this.authenticationInfo, new [] { this.toEncrypt });

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is string);
            Assert.AreNotEqual(actual.ToString(), this.toEncrypt);
            Assert.AreEqual(actual.ToString(), this.toDecrypt);
        }

        /// <summary>
        /// Tests that the Decrypt method should return an empty string with an empty string parameter
        /// </summary>
        [Test]
        public void DecryptShouldReturnEmtpyStringWithEmtpyString()
        {
            var decryptMethod = this.authenticationInfo.GetType()
                .GetInstanceMethod("Decrypt");

            var actual = decryptMethod.Invoke(this.authenticationInfo, new[] { string.Empty });

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is string);
            Assert.AreEqual(string.Empty, actual);
        }

        /// <summary>
        /// Tests that the Decrypt method should return an empty string with a null string parameter
        /// </summary>
        [Test]
        public void DecryptShouldReturnEmtpyStringWithNullString()
        {
            var decryptMethod = this.authenticationInfo.GetType()
                .GetInstanceMethod("Decrypt");

            var actual = decryptMethod.Invoke(this.authenticationInfo, new string[] { null });

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is string);
            Assert.AreEqual(string.Empty, actual);
        }

        /// <summary>
        /// Tests that the Decrypt method should return a correctly decrypted string
        /// </summary>
        [Test]
        public void DecryptShouldReturnDecryptedString()
        {
            var decryptMethod = this.authenticationInfo.GetType()
                .GetInstanceMethod("Decrypt");

            var actual = decryptMethod.Invoke(this.authenticationInfo, new [] { this.toDecrypt });

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is string);
            Assert.AreEqual(this.toEncrypt, actual);
        }
    }
}