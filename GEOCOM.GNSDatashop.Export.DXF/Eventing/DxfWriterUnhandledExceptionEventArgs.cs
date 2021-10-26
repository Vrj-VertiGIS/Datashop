using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Eventing
{
    public class DxfWriterUnhandledExceptionEventArgs : EventArgs
    {
        public DxfWriterUnhandledExceptionEventArgs(Exception ex, IEnumerable<string> filesWrittenSoFar)
        {
            Exception = ex;
            FilesWritten = filesWrittenSoFar;
        }

        public Exception Exception { get; private set; }

        public IEnumerable<string> FilesWritten { get; private set; }
    }
}
