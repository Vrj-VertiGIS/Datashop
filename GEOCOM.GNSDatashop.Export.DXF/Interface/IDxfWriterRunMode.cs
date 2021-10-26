using GEOCOM.GNSDatashop.Export.DXF.Common;

namespace GEOCOM.GNSDatashop.Export.DXF.Interface
{
    /// <summary>
    /// Optionally set/get run mode (interactive, server, batch)
    /// </summary>
    public interface IDxfWriterRunMode
    {
        DxfWriterRunMode RunMode { get; set; }
    }
}
