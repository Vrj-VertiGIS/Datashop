using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.EntityNames
{
    public class DxfEntityNames : Dictionary<string, int>
    {
        private static DxfEntityNames _instance = null;

        public DxfEntityNames() : base(new ListElementComparer()) { }

        public static DxfEntityNames Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new DxfEntityNames();
                return _instance;
            }
        }

        public new int this[string entityName]
        {
            get
            {
                int occ = -1;
                if (TryGetValue(entityName, out occ))
                    return occ;
                return 0;
            }
            set
            {
                base[entityName] = value;
            }
        }

        public static int Occurs(string entityName)
        {
            return Instance[entityName];
        }

        public static void SetOccurrance(string entityName, int nOcc)
        {
            Instance[entityName] = nOcc;
        }

        public new static void Clear()
        {
            ((Dictionary<string, int>)Instance).Clear();
        }

        private class ListElementComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
