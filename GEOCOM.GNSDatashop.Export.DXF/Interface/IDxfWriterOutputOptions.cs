using netDxf.Header;
using ESRI.ArcGIS.esriSystem;

namespace GEOCOM.GNSDatashop.Export.DXF.Interface
{
    public interface IDxfWriterOutputOptions
    {
        DxfVersion DxfVersion { get; set; }

        // Keep empty output files - i.e. files create by empty selection polygons
        bool KeepEmptyDxfFiles { get; set; }

        ITrackCancel CancelTracker { get; set; }

        IStepProgressor StepProgressor { get; set; }
    }
}
