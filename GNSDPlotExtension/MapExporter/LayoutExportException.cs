using System;

namespace GEOCOM.GNSD.PlotExtension.MapExporter
{
    /// <summary>
    /// This exception signals invalid data layers in a mxd map file.
    /// </summary>
    public class LayoutExportException : ApplicationException
    {
        public LayoutExportException(string message)
            : base(message)
        { }

        /// <summary>
        /// Comma separated names of the invalid layers.
        /// </summary>
        public string InvalidLayersNames
        {
            get { return Data["invalidLayersNames"] as string; }
            internal set { Data["invalidLayersNames"] = value; }
        }

        /// <summary>
        /// Path to the mxd map document that contains the invalid layers.
        /// </summary>
        public string MapFileMxdPath
        {
            get { return Data["mapFileMxdPath"] as string; }
            internal set { Data["mapFileMxdPath"] = value; }
        }

    }
}