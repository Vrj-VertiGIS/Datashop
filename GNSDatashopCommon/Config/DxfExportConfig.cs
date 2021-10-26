using System.Collections.Generic;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    [XmlRoot("dxf")]
    public class DxfExportConfig
    {
        [XmlElement("dxfexport")]
        public List<DxfExportInfo> DxfExports { get; set; }
    }

    public class DxfExportInfo
    {
        [XmlAttribute("lyrFilePath")]
        public string LyrFilePath { get; set; }

        [XmlAttribute("visibleOnly")]
        public bool VisibleOnly { get; set; }

        [XmlAttribute("saveSymbol")]
        public bool SaveSymbols { get; set; }

        [XmlAttribute("symbolAsFont")]
        public bool SymbolAsFont { get; set; }

        [XmlAttribute("clipHoles")]
        public bool ClipHole { get; set; }

        [XmlAttribute("clipBoundaries")]
        public bool ClipBoundaries { get; set; }

        [XmlAttribute("referenceScale")]
        public double RefScale { get; set; }

        [XmlAttribute("textLineSpacing")]
        public double LineSpacing { get; set; }

        [XmlAttribute("blockPath")]
        public string CsvBlockPathList { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }
    }
}