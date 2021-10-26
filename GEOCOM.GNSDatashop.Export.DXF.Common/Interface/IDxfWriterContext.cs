using System;
using System.Collections.Generic;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.Common.Clipping;
using netDxf;
using netDxf.Header;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Interface
{
    /// <summary>
    /// Internal - read-only - dxf writer members
    /// </summary>
    public interface _IDxfWriterContext
    {
        IDisplay RenderDisplay { get; }     // ArcMap render display
        IntPtr HDC { get; }                 // Compatible memory hDC based on RenderDisplay's hWND
        Graphics Graphics { get; }          // Graphics object drawing to memory context HDC

        IEnumerable<ILayer> Layers { get; }
        IEnumerable<ILayer> VisibleOnlyLayers { get; }

        RegionOfInterest RegionOfInterest { get; }
        bool UseFeatureSelection { get; }

        DxfDocument DxfDocument { get; }
        DxfVersion DxfVersion { get; }
        string DxfFileName { get; }

        double MapReferenceScale { get; }
        double MapCurrentScale { get; }

        CancelTrackerDummy CancelTracker { get; }
        StepProgressorDummy StepProgressor { get; }

        IList<string> FilesWritten { get; }

        DxfWriterRunMode RunMode { get; }
    }
}