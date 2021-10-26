using GEOCOM.GNSDatashop.Export.DXF.Common;
using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Eventing
{
    public class DxfWriterSuccessEventEventArgs : EventArgs
    {
        public DxfWriterSuccessEventEventArgs(IList<string> filesWritten, ProcessedLayersInfo layersInfo)
        {
            FilesWritten = filesWritten;
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
