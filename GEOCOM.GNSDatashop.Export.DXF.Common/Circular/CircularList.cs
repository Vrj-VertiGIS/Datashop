using System.Collections;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Circular
{
    public class CircularList<T> : IEnumerable<T>
    {
        private List<T> _data;

        public CircularList() { _data = new List<T>(); }

        public CircularList(IEnumerable<T> data) { _data = new List<T>(data); }

        public IEnumerator<T> GetEnumerator()
        {
            return new CircularEnumerator<T>(_data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CircularEnumerator<T>(_data);
        }
    }
}
