using System;
using System.Collections;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Circular
{
    public class CircularEnumerator<T> : IDisposable, IEnumerator<T>
    {
        private IEnumerator<T> _data;   

        public CircularEnumerator(IEnumerable<T> data)
        {
            _data = data.GetEnumerator();
        }

        public T Current => _data.Current;

        object IEnumerator.Current => _data.Current;

        public bool MoveNext()
        {
            if (!_data.MoveNext())
            {
                Reset();
                if (!_data.MoveNext())
                    throw new InvalidOperationException();
            }
            return true;
        }

        public void Reset()
        {
            _data.Reset();
        }

        public void Dispose()
            => DisposeManaged();

        private void DisposeManaged()
            => _data?.Dispose();

    }
}
