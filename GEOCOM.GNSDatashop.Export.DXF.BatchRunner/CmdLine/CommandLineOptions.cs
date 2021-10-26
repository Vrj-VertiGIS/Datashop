using CommandLine;

using netDxf.Header;

namespace GEOCOM.GNSDatashop.Export.DXF.BatchRunner.CmdLine
{

    public class CmdLineOptions
    {
        // Not functional by now - don't know why
        // private static StoLanguage _lng = new StoLanguage() { AppName = "GEOCOM.GNSDatashop.Export.DXF.dll" };

        public CmdLineOptions()
        {
           // TranslateAttributes();

            MapScale = null;
            RestricToVisibleLayers = null;
            Binary = null;
        }

        // [Option('m', "mxd", Required = true, HelpText = "20110Name of map file (.mxd) to export data from")]
        [Option('m', "mxd", Required = true, HelpText = "Name of map file (.mxd) to export data from")]
        public string Mxd { get; set; }

        // [Option('d', "dxf" , Required = true, HelpText = "20112Name of DXF file(s) to be written. The full path (drive, directory, file name, extension) in case of a single export, the full path(drive, directory, extension in case of a multiple export (see refionOfInterest) which will be appended a 3-digit sequence number between file name and extension")]
        [Option('d', "dxf", Required = true, HelpText = "Name of dxf file(s) to be written. If no selection layer given (see parameter -s), the name of one single output file. If a selection layer is also specified, the output file name will be split and a 3-digit running number will be inserted between the file name and it's extension. So when specifying data.dxf, files data_001.dxf, data_002.dxf, data_003.dxf... will be the result. Each one of these output files will contain the data selected by a single polygon in the selection layer.")]
        public string Dxf { get; set; }

        [Option('n', "dxfversion", Required = false, Default = DxfVersion.AutoCad2010, HelpText = "Version of the DXF format to write")]
        public DxfVersion DxfVerion { get; set; }

        // [Option('s', "selectionlayer", Required = false, Default="", HelpText = "20114Name of a layer specifying export selections")]
        [Option('s', "selectionlayer", Required = false, HelpText = "Name of a layer specifying export selections")]
        public string SelectionLayer { get; set; }

        // [Option('a', "maskinglayer", Required = false, Default="", HelpText = "20116Name of a leyer used to mask out regions in exported areas")]
        [Option('a', "maskinglayer", Required = false, HelpText = "Name of a layer used to mask out regions in exported areas")]
        public string MaskingLayer { get; set; }

        // [Option('v', "restricttovisible",  Required = false, Default = false, HelpText = "20118Restrict to visible layers (True/false - default=True)")]
        [Option('v', "restricttovisible",  Required = false, Default = false, HelpText = "Restrict to visible layers (true/false)")]
        public bool? RestricToVisibleLayers { get; set; }

        // [Option('l', "mapscale", Required = false, HelpText = "20120Map scale (1:n), i.E. 500 for 1:500, 1000 for 1:1000 etc. If not specified, the reference scale of the map document will be used")]
        [Option('l', "mapscale", Required = false, HelpText = "Map scale (1:n), i.E. 500 for 1:500, 1000 for 1:1000 etc. If not specified, the reference scale of the map document will be used")]
        public double? MapScale { get; set; }

        // [Option('b', "binary", Required = false, Default = false, HelpText = "Write binary dxf files")]
        [Option('b', "binary", Required = false, Default = false, HelpText = "Write binary dxf files")]
        public bool? Binary { get; set; }

        // [Option('o', "log", Required = false, HelpText = "20160Name of a log file (if logging should not be output to standard errout)")]
        [Option('o', "log", Required = false, HelpText = "Name of a log file")]
        public string Log { get; set; }

        #region language text helpers

        /*
        private static void TranslateAttributes()
        {
            var props = typeof(CmdLineOptions).GetProperties();
            foreach (var prop in props)
            {
                var oa = prop.GetCustomAttribute<OptionAttribute>();
                if (null != oa)
                    TranslateOptionAttribute(oa);
            }
        }

        private static void TranslateOptionAttribute(OptionAttribute oa)
        {
            Match numberMatch = Regex.Match(oa.HelpText, @"^\d{1,}");
            if (numberMatch.Success)
                oa.HelpText = _lng.LoadStr(int.Parse(numberMatch.Value), oa.HelpText.Substring(numberMatch.Length));
        }
        */

        #endregion
    }
}
