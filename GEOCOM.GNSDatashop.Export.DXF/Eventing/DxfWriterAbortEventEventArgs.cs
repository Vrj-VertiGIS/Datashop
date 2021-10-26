using GEOCOM.GNSDatashop.Export.DXF.Common;
using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Eventing
{
    public class DxfWriterAbortEventEventArgs : EventArgs
    {
        public DxfWriterAbortEventEventArgs(IList<string> filesWrittenSoFar, ProcessedLayersInfo layersInfo)
        {
            FilesWritten = filesWrittenSoFar;
            _layersInfo = layersInfo;
        }

        private ProcessedLayersInfo _layersInfo;

        public IEnumerable<string> FilesWritten { get; private set; }

        public ErroneousLayersInfo ErroneousLayers
            => _layersInfo.ErroneousLayers;
        public ExportedLayersInfo ExportedLayers
            => _layersInfo.ExportedLayers;
    }
}
