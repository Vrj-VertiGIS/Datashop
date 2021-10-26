using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSD.PlotExtension.Layout
{
    public class OriginalTextEnumerator : IEnumerator<String>
    {
        private List<String> _originalTextList;
        private int _currentIndex;
        private String currentText;


        public OriginalTextEnumerator()
        {
            _originalTextList = new List<string>();
            _currentIndex = -1;
            currentText = default(String);
        }

        public void AddText(String text)
        {
            _originalTextList.Add(text);
        }

        public bool MoveNext()
        {
            //Avoids going beyond the end of the collection.
            if (++_currentIndex >= _originalTextList.Count)
            {
                return false;
            }
            else
            {
                // Set current box to next item in collection.
                currentText = _originalTextList[_currentIndex];
            }
            return true;
        }

        public void Reset() { _currentIndex = -1; }

        void IDisposable.Dispose() { }

        public String Current
        {
            get { return currentText; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

    }
}
