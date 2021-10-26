using System;
using System.Collections;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    /// <summary>
    /// Make shure the embedded enumerator's Current property returns
    /// null in case the enumerator is not on a valid position 
    /// (before the first or after the last element).
    /// </summary>
    public class LabelTextTokens : IEnumerator<LabelTextToken>
    {
        private IEnumerator<LabelTextToken> _tokens;

        public LabelTextTokens(IEnumerator<LabelTextToken> tokens)
        {
            _tokens = tokens;
            ValidData = _tokens.MoveNext();
        }

        public bool ValidData { get; private set; } = false;

        public LabelTextToken Current => (ValidData) ? _tokens.Current : null;

        object IEnumerator.Current => (ValidData) ? ((IEnumerator)_tokens).Current : null;

        public void Dispose()
        {
            _tokens.Dispose();
        }

        public bool MoveNext()
        {
            return (ValidData) ? ValidData = _tokens.MoveNext() : false;
        }

        public void Reset()
        {
            throw new NotImplementedException("Since this is a dynamic collection created by yield return... - Reset() is not implemented");
        }

    }
}
