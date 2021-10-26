using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSD.Common.Model
{
    /// <summary>
    /// This is a copy of the enum GEOCOM.TDE.Enumerations.OutputFormat!!!
    /// Both are used across the solution.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// This is used if no flag is set -> no export will be performed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The target type is an MDB.
        /// </summary>
        pgdb = 1,

        /// <summary>
        /// This flag has to be set if ou wan't to get a filegeodatabase.
        /// </summary>
        fgdb = 2,

        /// <summary>
        /// Here you get a DXF
        /// </summary>
        DXF = 4,

        /// <summary>
        /// This generates a PDF
        /// </summary>
        PDF = 8
    }

    public class TdeExtensionConst
    {
        public const string PROGID = "teildatenexport";
        public const string GUID = "todo.guid.fuer.extension";
    }
}
