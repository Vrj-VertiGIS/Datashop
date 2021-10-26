using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Interface
{
    public interface IDxfWriterOutputInfo
    {
        IEnumerable<string> FilesWritten { get; }
    }
}
