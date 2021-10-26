using System;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// Code behind class for the Export Type Selector
    /// </summary>
    /// <seealso cref="GEOCOM.GNSD.Web.Controls.RequestUserControl" />
    public partial class ExportTypeSelector : RequestUserControl
    {
        /// <summary>
        /// Gets a value indicating whether [DXF export].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [DXF export]; otherwise, <c>false</c>.
        /// </value>
        public bool DxfExport
        {
            get
            {
                return chkDxfExport.Checked;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}