using System;
using System.IO;
using System.Xml.Serialization;

namespace GNSDatashopAdmin.Helpers
{
    public class XmlSerializer<T>
    {
        private string _errorMessage;

        public T Deserialize(string filename)
        {
            T deserializedObject;
            var serializer = new XmlSerializer(typeof(T));
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                serializer.UnknownAttribute += this.SerializerUnknownAttribute;
                serializer.UnknownElement += this.SerializerUnknownElement;
                serializer.UnknownNode += this.SerializerUnknownNode;
                this._errorMessage = string.Empty;
                deserializedObject = (T)serializer.Deserialize(fileStream);
                if (!string.IsNullOrEmpty(this._errorMessage))
                    throw new Exception(this._errorMessage);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return deserializedObject;
        }

        private void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
        {
            this._errorMessage += string.Format(
                "Unexpected node '{0}' at line {1}, column {2}. ",
                (e.Name == "#text") ? e.Text : e.Name,
                e.LineNumber,
                e.LinePosition);
        }

        private void SerializerUnknownElement(object sender, XmlElementEventArgs e)
        {
            this._errorMessage += string.Format("Unexpected element '{0}' at line {1}, column {2}. ", e.Element.Name, e.LineNumber, e.LinePosition);
        }

        private void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            this._errorMessage += string.Format(
                "Unexpected attribute '{0}' at line {1}, column {2}. ", e.Attr.Name, e.LineNumber, e.LinePosition);
        }
    }
}
