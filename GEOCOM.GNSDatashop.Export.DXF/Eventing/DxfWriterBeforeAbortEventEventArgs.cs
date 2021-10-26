using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Eventing
{
    public class DxfWriterBeforeAbortEventEventArgs : EventArgs
    {
        public bool CancelAbort { get; set; }
    }
}
