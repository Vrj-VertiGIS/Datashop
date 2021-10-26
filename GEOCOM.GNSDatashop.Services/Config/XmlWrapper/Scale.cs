using System.Xml.Serialization;
using GEOCOM.GNSDatashop.Model;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
    public class Scale
    {
        [XmlIgnore]
        public KeyValuesPair[] RequestScales { get; set; }

        [XmlElement("role")]
        public Role[] MockUpFieldForRequestScales
        {
            get
            {
                if (RequestScales == null) return null;

                Role[] roles = new Role[RequestScales.Length];
                for (int i = 0; i < roles.Length; i++)
                {
                    KeyValuesPair requestScales = RequestScales[i];
                    Role role = new Role();
                    role.Name = requestScales.Key;
                    role.KeyTextPairs = requestScales.Value as KeyTextPair[];
                    roles[i] = role;
                }
                return roles;
            }

            set
            {
                RequestScales = new KeyValuesPair[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    Role role = value[i];
                    KeyValuesPair pair = new KeyValuesPair();
                    pair.Key = role.Name;
                    pair.Value = role.KeyTextPairs;
                    RequestScales[i] = pair;
                }
            }
        }
    }
}
