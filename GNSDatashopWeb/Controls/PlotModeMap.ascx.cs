using System;
using System.Globalization;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web.Controls
{
    public partial class PlotModeMap : MapBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the rotation slider behaviour identifier.
        /// </summary>
        /// <value>
        /// The rotation slider behaviour identifier.
        /// </value>
        public string RotationSliderBehaviourId { get; set; } 

        #endregion


        #region Page LifeCycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
	        if (!Page.IsPostBack)
	        {
		        HandleLoadingPreviousMapExtents();
	        }
        } 

        #endregion
       }
}