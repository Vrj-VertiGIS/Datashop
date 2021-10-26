using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class DxfBlockNames : Dictionary<string, int>
    {
        private static DxfBlockNames _instance = null;

        public DxfBlockNames() : base(new ListElementComparer()) { }

        public static DxfBlockNames Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new DxfBlockNames();
                return _instance;
            }
        }

        public new int this[string blockName]
        {
            get
            {
                int occ = -1;
                if (TryGetValue(blockName, out occ))
                    return occ;
                return 0;
            }
            set
            {
                base[blockName] = value;
            }
        }

        public static int Occurs(string blockName)
        {
            return Instance[blockName];
        }

        public static void SetOccurrance(string blockName, int nOcc)
        {
            Instance[blockName] = nOcc;
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
