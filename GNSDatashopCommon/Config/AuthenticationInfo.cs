using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    public class AuthenticationInfo : IXmlSerializable
    {
        private string someGuid;
        private byte[] salt4 = new byte[8];
        private byte[] salt5;
        private byte[] salt6 = new byte[8];

        public AuthenticationInfo()
        {
            // init passwords and salts
            someGuid = "{9F97C0E1-6D16-48f6-BE1C-4B9E22A6166A}";
            salt5 = new Guid("{B5A3D62E-85C5-4a2c-BD6E-3C4739B1974D}").ToByteArray();
            byte[] temp = new Guid("{7BDBAD09-DAF8-4af6-8F55-B143195FDA14}").ToByteArray();
            Array.Copy(temp, 0, salt4, 0, 8);    // salt4 and 6 are only 64bit            
            Array.Copy(temp, 8, salt6, 0, 8);
        }

        public string User { get; set; }

        public string EncryptedPassword { get; set; }

        public string Domain { get; set; }

        // indicates if a password is still configured with cleartext to warn the user
        public bool ConfiguredWithClearTextPassword { get; set; }

        public string Password
        {
            get
            {
                return Decrypt(EncryptedPassword);
            }
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            User = reader.GetAttribute("loginname");
            Domain = reader.GetAttribute("domain");
            EncryptedPassword = reader.GetAttribute("password");
            string cleartextPassword = reader.GetAttribute("cleartextpassword");

            if (!string.IsNullOrEmpty(cleartextPassword))
            {
                ConfiguredWithClearTextPassword = true;
                EncryptedPassword = Encrypt(cleartextPassword);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("loginname", User);
            writer.WriteAttributeString("domain", Domain);
            writer.WriteAttributeString("password", EncryptedPassword);
        }

        #endregion

        private string Decrypt(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                // create a writebuffer
                MemoryStream ms = new MemoryStream(2000);

                // create de decryptor
                ICryptoTransform transform = GetCryptoTransformation(false);
                CryptoStream cryptoStream = new CryptoStream(ms, transform, CryptoStreamMode.Write);

                // create a decoder
                ICryptoTransform decode = new FromBase64Transform();
                CryptoStream fromBase64Stream = new CryptoStream(cryptoStream, decode, CryptoStreamMode.Write);

                // create a buffer
                byte[] buffer = Encoding.UTF8.GetBytes(s);

                fromBase64Stream.Write(buffer, 0, buffer.Length);
                fromBase64Stream.FlushFinalBlock();
                fromBase64Stream.Close();
                cryptoStream.Close();
                ms.Close();

                buffer = ms.ToArray();
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return string.Empty;
        }

        private ICryptoTransform GetCryptoTransformation(bool encrypt)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.IV = salt5;
            aes.Key = new PasswordDeriveBytes(someGuid, salt5).CryptDeriveKey("RC2", "MD5", 128, salt6);

            if (encrypt) return aes.CreateEncryptor();
            return aes.CreateDecryptor();
        }

        private string Encrypt(string s)
        {
            // buffer to store result
            MemoryStream ms = new MemoryStream(2000);

            // create an encoder to transform to the binary code to base64
            ICryptoTransform encode = new ToBase64Transform();
            CryptoStream toBase64Stream = new CryptoStream(ms, encode, CryptoStreamMode.Write);

            // create the encryptor
            ICryptoTransform transform = GetCryptoTransformation(true);
            CryptoStream cryptoStream = new CryptoStream(toBase64Stream, transform, CryptoStreamMode.Write);

            byte[] buffer = Encoding.UTF8.GetBytes(s);
            cryptoStream.Write(buffer, 0, buffer.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            toBase64Stream.Close();
            ms.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}