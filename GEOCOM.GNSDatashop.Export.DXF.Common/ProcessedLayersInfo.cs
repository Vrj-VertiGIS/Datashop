using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class ProcessedLayersInfo
    {
        private ExportedLayersInfo _exportedLayers = null;
        public ExportedLayersInfo ExportedLayers
            => _exportedLayers ?? (_exportedLayers = new ExportedLayersInfo());

        private ErroneousLayersInfo _erroneousLayers = null;
        public ErroneousLayersInfo ErroneousLayers
            => _erroneousLayers ?? (_erroneousLayers = new ErroneousLayersInfo());

        public void Append(ProcessedLayersInfo other)
        {
            ExportedLayers.AddRange(other.ExportedLayers);
            ErroneousLayers.AddRange(other.ErroneousLayers);
        }

        public void AddErroneousLayer(ILayer layer, Exception ex, string message)
            => ErroneousLayers.Add(new ErroneousLayerInfo(layer, ex) { Message = message });

        public void AddErroneousLayer(ILayer layer, Exception ex)
            => ErroneousLayers.Add(new ErroneousLayerInfo(layer, ex));

        public void AddErroneousLayer(ILayer layer)
            => ErroneousLayers.Add(new ErroneousLayerInfo(layer));

        public void AddErroneousLayer(IEnumerable<IElement> drawing, Exception ex, string message)
            => ErroneousLayers.Add(new ErroneousLayerInfo(drawing, ex) { Message = message });

        public void AddErroneousLayer(IEnumerable<IElement> drawing, Exception ex)
            => ErroneousLayers.Add(new ErroneousLayerInfo(drawing, ex));

        public void AddErroneousLayer(IEnumerable<IElement> drawing)
            => ErroneousLayers.Add(new ErroneousLayerInfo(drawing));

        public void AddExportedLayer(ILayer layer, double dotsToMeter)
            => ExportedLayers.Add(new ExportedLayerInfo(layer) { DotsToMeter = dotsToMeter });

        public void AddExportedLayer(IEnumerable<IElement> drawing, double dotsToMeter)
            => ExportedLayers.Add(new ExportedLayerInfo(drawing) { DotsToMeter = dotsToMeter });

        public void Clear()
        {
            ExportedLayers.Clear();
            ErroneousLayers.Clear();
        }

    }
}
