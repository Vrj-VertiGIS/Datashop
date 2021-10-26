using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.Interface
{
    public interface IDxfWriter
    {
        /// <summary>
        /// Write multiple DXF files. One File per (Multipart-)Geometry in regionOfInterest. dxfTemplateFileName is used as a file name template.
        /// occasionally (when there is more than one geometry to export) the running file number (_0001, _0002,...) is appended.
        /// </summary>
        /// <param name="dxfTemplateFileName">A fully qualified dxf file name to be written</param>
        /// <param name="visibleOnly">Export visible layers only</param>
        /// <param name="regionOfInterest">A collection of regions (Polygon geometries) to be exported - one per dxf file.</param>
        /// <param name="maskGeometries">Mask out any feature covered by one of the polygons within this.</param>
        void WriteDXFByTemplate(string dxfTemplateFileName, bool visibleOnly, IEnumerable<IGeometry> regionOfInterest, IGeometry maskGeometries);

        /// <summary>
        /// Export the contents of a map. The data to be exported is specified by beeing contained within the (multipart) polygon
        /// given by regionOfInterest.
        /// </summary>
        /// <param name="dxfFileName">The fully qualified name of the output file to be written.</param>
        /// <param name="visibleOnly">Export visible layers only</param>
        /// <param name="regionOfInterest">A region (single- or multipart polygon) specifying the data to export.</param>
        /// <param name="maskGeometries">Mask out any feature covered by one of the polygons within this.</param>
        void WriteSingleDXF(string dxfFileName, bool visibleOnly, IGeometry regionOfInterest, IGeometry maskGeometries);
    }
}