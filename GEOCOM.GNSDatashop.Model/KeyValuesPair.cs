using System;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
    public class KeyValuesPair
    {
        public KeyValuesPair(){}

        public KeyValuesPair(string key, KeyTextPair[] value)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public KeyTextPair[] Value { get; set; }
    }
}
