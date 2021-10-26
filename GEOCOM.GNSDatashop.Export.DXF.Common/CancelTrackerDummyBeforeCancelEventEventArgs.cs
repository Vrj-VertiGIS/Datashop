using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class CancelTrackerDummyBeforeCancelEventEventArgs : EventArgs
    {
        public bool AbortCancelRequest { get; set; }
    }
}
