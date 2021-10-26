using System;

using GEOCOM.GNSDatashop.Export.DXF.Eventing;

namespace GEOCOM.GNSDatashop.Export.DXF.Interface
{
    public interface IDxfWriterEvents
    {
        event EventHandler<DxfWriterStartEventEventArgs> OnStart;
        event EventHandler<DxfWriterSuccessEventEventArgs> OnSuccess;
        event EventHandler<DxfWriterAbortEventEventArgs> OnAbort;
        event EventHandler<DxfWriterBeforeAbortEventEventArgs> OnBeforeAbort;
        event EventHandler<EventArgs> OnNothingDone;
        event EventHandler<DxfWriterUnhandledExceptionEventArgs> OnUnhandledException;
    }
}
