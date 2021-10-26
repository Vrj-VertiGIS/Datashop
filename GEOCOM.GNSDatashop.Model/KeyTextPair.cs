using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
    public class KeyTextPair : IComparable<KeyTextPair>
    {
        public KeyTextPair()
        {
        }

        public KeyTextPair(string key, string text)
        {
            Key = key;
            Text = text;
        }

        [XmlAttribute("key")]
        [DataMember]
        public string Key { get; set; }

        [XmlAttribute("text")]
        [DataMember]
        public string Text { get; set; }

        #region IComparable<KeyTextPair> Members

        public int CompareTo(KeyTextPair other)
        {
            return Text.CompareTo(other.Text);
        }

        #endregion
    }
}
