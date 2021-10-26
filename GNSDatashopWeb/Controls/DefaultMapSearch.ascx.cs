using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.AddressSearch;
using System.Linq;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// Encapsulates all configured searches and shows an error message when the search extensions service is not available.
    /// </summary>
    public partial class DefaultMapSearch : RequestUserControl
    {
        #region Page Lifecycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!DatashopWebConfig.Instance.MapSearch.DefaultEnabled)
                return;

            this.PopulateSearchControls();
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Populates the search controls.
        /// </summary>
        private void PopulateSearchControls()
        {
            var searches = this.GetConfiguredSearchDefinitions();

            if (searches == null)
                this.lblNoSearch.Visible = true;
            else
            {
                this.lblNoSearch.Visible = false;

                foreach (var geoSearchDef in searches)
                {
                    var search = this.LoadControl("~/Controls/SearchControl.ascx") as SearchControl;
                    
                    if (search != null)
                    {
                        search.ID = geoSearchDef.Name;
                        search.GeoSearchDef = geoSearchDef;
                        search.CommonSearchControl = this;

                        searchControls.Controls.Add(search);
                    }
                }
            }
        }

        /// <summary>
        /// Get the search list with the datashop web service
        /// wia cached the list in the session to spare server round trips
        /// </summary>
        /// <returns></returns>
        private IEnumerable<GeoSearchDef> GetConfiguredSearchDefinitions()
        {
            const string searchListSessionName = "SEARCHLIST";

            var sessionList = Session[searchListSessionName];

            if (sessionList is SearchListNotavailable)
                return null;

            if (sessionList is List<GeoSearchDef>)
                return (List<GeoSearchDef>)sessionList;

            var searches = new List<GeoSearchDef>();

            try
            {
                var definition = DatashopService.Instance.AddressSearchService.GetSearchDefinitions();

                if (definition != null && definition.Searches != null)
                    searches.AddRange(definition.Searches);

                Session.Add(searchListSessionName, searches);
            }
            catch (Exception ex)
            {
                LogError("GetConfiguredSearchDefinitions failed: " + ex.Message, ex);

                Session.Add(searchListSessionName, new SearchListNotavailable(ex));

                searches = null;
            }

            return searches;
        } 

        #endregion

        public class SearchListNotavailable
        {
            public Exception Ex { get; set; } 

            public SearchListNotavailable(Exception ex)
            {
                this.Ex = ex;
            }
        }
    }
}