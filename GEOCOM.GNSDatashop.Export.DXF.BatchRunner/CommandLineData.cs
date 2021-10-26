using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.BatchRunner
{
    public class CommandLineData
    {
        public IMap Map { get; set}

        public string DXFFileFullPath { get; set; }

        public IEnumerable<IGeometry> ExportVisibleOnlyLayers { get; set; }

        public IGeometry Mask { get; set; }            
    }
}
