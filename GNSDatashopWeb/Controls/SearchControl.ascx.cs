using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.AddressSearch;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// This corresponds to one GeoFind
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public GeoSearchDef GeoSearchDef;

        private const String FINAL = "final";

        private const String INITIAL = "initial";

        private const String EDITBOXPREFIX = "¢";

        protected String IMAGENAME = "maximize.png";

        private Dictionary<string, Control> controls = new Dictionary<string, Control>();

        public Control CommonSearchControl { get; set; }

        public string CommonSearchControlClientId 
        { 
            get
            {
                return this.CommonSearchControl == null ? string.Empty : this.CommonSearchControl.ClientID;
            } 
        }

        // log4net
        private IMsg _log = new Msg(typeof (SearchControl));

       

        protected void Page_Init(object sender, EventArgs e)
        {
            _log.DebugFormat("Searchcontrol-init sender {0}", sender);

            if (GeoSearchDef == null)
            {
                throw new ArgumentException("Geofind property for SearchControll was empty. Please set the geoSearchDef property before adding this controll to a page.");
            }
            this.CreateControls();

            if (!Page.IsPostBack)
                SelectedIndexChanged(null, null);
        }

        private void CreateControls()
        {
            _log.DebugFormat("Constructing gui for search {0}.", GeoSearchDef.Name);
            try
            {
                var hgc = new HtmlGenericControl();
                hgc.TagName = "br";
                Title.Text = WebLanguage.LoadStr(GeoSearchDef.LanguageID, GeoSearchDef.Title);

                SetDisplayStyle();

                foreach (var control in GeoSearchDef.Controls)
                {



                    var upPanel = new UpdatePanel
                                      {
                                          ID = control.Name + "_updatePanel",
                                          UpdateMode = UpdatePanelUpdateMode.Conditional,
                                          RenderMode = UpdatePanelRenderMode.Inline,
                                          ChildrenAsTriggers = true,
                                         
                                      };
                    var containerStart = new LiteralControl { Text = "<div class='requestpage-control'>" };
                    placeholder.Controls.Add(containerStart);

                    var lableTemp = new Label
                                        {
                                            Text = WebLanguage.LoadStr(control.LanguageID, control.Title),
                                            CssClass = "requestpage-label"
                                        };

                    placeholder.Controls.Add(lableTemp);

                    placeholder.Controls.Add(upPanel);

                    Control field = null;

                    if (control.ControlType == SearchControlType.Edit)
                    {
                        var txbTemp = new TextBox
                                          {
                                              AutoPostBack = false,
                                              EnableViewState = true,
                                              CssClass = "requestpage-edit",
                                              ID = EDITBOXPREFIX + control.Name
                                          };
                        
                        txbTemp.Attributes.Add("onkeydown", "if(event.keyCode==13)return HandleEnterKey(\"" + ID + "\");");
                        txbTemp.Attributes.Add("onkeyup", "if(event.keyCode==13)return false;");                        

                        var imgBtn = new ImageButton
                                         {
											 ImageUrl = "~/images/img_icon_search.png",
                                             ImageAlign = ImageAlign.AbsMiddle,
                                             ToolTip = WebLanguage.LoadStr(3980, "Update fields"),
                                             ID = control.Name,
                                             CssClass = "img_search_updater"
                                         };

                        imgBtn.Click += SelectedIndexChanged;

                        field = txbTemp;

                        upPanel.ContentTemplateContainer.Controls.Add(field);                        
                        upPanel.ContentTemplateContainer.Controls.Add(imgBtn);

                    }
                    else
                    {
                        var ddlTemp = new DropDownList
                                          {
                                              CssClass = "requestpage-combobox",
                                              AutoPostBack = true,
                                              EnableViewState = true,
                                              ID = control.Name
                                          };

                        ddlTemp.SelectedIndexChanged += SelectedIndexChanged;
                        field = ddlTemp;
                        //UpdatePanelSearch.ContentTemplateContainer.Controls.Add(field);
                        upPanel.ContentTemplateContainer.Controls.Add(field);

                        // uncomment this line to improve search speed in IE (experimental)
                        // RegisterScript(ddlTemp);
                    }

                    placeholder.Controls.Add(new LiteralControl { Text = "</div>" });


                    controls.Add(field.ID.Contains(EDITBOXPREFIX) ? field.ID.TrimStart(Convert.ToChar(EDITBOXPREFIX)) : field.ID, field);
                    //UpdatePanelSearch.ContentTemplateContainer.Controls.Add(hgc);
                    placeholder.Controls.Add(hgc);


                }
                var lbl = new Label { Width = 186, Text = string.Empty };
                var btnFinaly = new Button { ID = FINAL, Text = WebLanguage.LoadStr(3981, "Zoom to object") };
                btnFinaly.Attributes.Add("style", "float:right; margin: 10px 11px 0 0; width:150px;");
                btnFinaly.Click += SelectedIndexChanged;

                //create UpdatePanel for zoomScript
                var upPanelZoom = new UpdatePanel
                                      {
                                          ID = GeoSearchDef.Name + "_zoomUpdatePanel",
                                          UpdateMode = UpdatePanelUpdateMode.Conditional,
                                          RenderMode = UpdatePanelRenderMode.Inline,
                                          ChildrenAsTriggers = true
                                      };

                upPanelZoom.ContentTemplateContainer.Controls.Add(btnFinaly);

                placeholder.Controls.RemoveAt(placeholder.Controls.Count - 1); // remove last <br>
                UpdatePanelSearch.ContentTemplateContainer.Controls.AddAt(UpdatePanelSearch.ContentTemplateContainer.Controls.Count, lbl);
                UpdatePanelSearch.ContentTemplateContainer.Controls.AddAt(UpdatePanelSearch.ContentTemplateContainer.Controls.Count, upPanelZoom);
            }
            catch (Exception exp)
            {
                _log.Error("Could not create gui for search " + GeoSearchDef.Title, exp);
                Title.Text = GeoSearchDef.Title + " is not well defined.";
            }
        }

        public void SelectedIndexChanged(object sender, EventArgs e)
        {

            _log.DebugFormat("Executing search for {0}", GeoSearchDef.Title);

            var timer = _log.StartTimeMeasure(MsgLevel.Debug, "Starting time measure of searching.");

            var geosearch = new GeoSearch();
            try
            {
                // fill values into geoSearchObject
                geosearch.Name = ID;
                if (sender == null)
                {
                    geosearch.RequestControlName = INITIAL;
                }
                else 
                {
                    // if(sender.GetType() == typeof(System.Web.UI.Control))
                    geosearch.RequestControlName = (sender as Control).ID;
                }
                geosearch.Controls = new GNSDatashop.Model.AddressSearch.SearchControl[controls.Count];

                var enumCtrls = controls.GetEnumerator();
                int i = 0;
                while (enumCtrls.MoveNext())
                {
                    geosearch.Controls[i] = new GNSDatashop.Model.AddressSearch.SearchControl();
                    geosearch.Controls[i].Name = enumCtrls.Current.Key;
                    if (enumCtrls.Current.Value.GetType() == typeof(System.Web.UI.WebControls.TextBox))
                    {
                        geosearch.Controls[i].Key = (enumCtrls.Current.Value as TextBox).Text;
                    }
                    else
                    {
                        geosearch.Controls[i].Key = (enumCtrls.Current.Value as DropDownList).SelectedValue;
                    }
                    i++;
                }
            }
            catch (Exception exp)
            {
                _log.Error("Could not read selected values in search " + GeoSearchDef.Title, exp);
                Title.Text = "Could not read values";

            }

            // Now send geosearch to webservice and process result;
            try
            {
                var t = _log.StartTimeMeasure(MsgLevel.Debug, "Starting timemeasure of search request to the server.");
                geosearch = DatashopService.Instance.AddressSearchService.PerformGeoSearch(geosearch);
                _log.StopTimeMeasure(MsgLevel.Debug, t, "Time taken for the trip to the searchserver and back : ");
            }
            catch (Exception exp)
            {
                _log.Error("Could not execute search for " + GeoSearchDef.Title, exp);
                Title.Text = "Service not available";

            }

            // Read new values and add them to the controlls
            try
            {
                if (geosearch.RequestControlName != FINAL)
                {
                    foreach (var control in geosearch.Controls)
                    {
                        if (control.QueryResult != null)
                        {
                            if (controls[control.Name].GetType() == typeof(TextBox))
                            {
                                (controls[control.Name] as TextBox).Text = "";
                                foreach (var pair in control.QueryResult)
                                {
                                    (controls[control.Name] as TextBox).Text = pair.Text;
                                }
                            }

                            else if (controls[control.Name].GetType() == typeof(DropDownList))
                            {
                                (controls[control.Name] as DropDownList).Items.Clear();

                                (controls[control.Name] as DropDownList).Items.Add(string.Empty);
                                foreach (var pair in control.QueryResult)
                                {
                                    (controls[control.Name] as DropDownList).Items.Add(new ListItem(pair.Text, pair.Id));
                                }
                            }

                            // get UpdatePanel and update it
                            if (controls[control.Name].Parent.Parent is UpdatePanel)
                            {
                                (controls[control.Name].Parent.Parent as UpdatePanel).Update();
                            }
                            RegisterScript(controls[control.Name]);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                _log.Error("Could not update values in gui for search " + GeoSearchDef.Title, exp);
                Title.Text = "Reading search result failed";

            }
            if (geosearch.RequestControlName == FINAL && geosearch.QueryResult != null)
            {
                if (geosearch.QueryResult.Count() > 1)
                {
                    Page.ShowMessage(WebLanguage.LoadStr(3982, "Found multiple results. The first will be shown"));
                }
                if (geosearch.QueryResult.Count() < 1)
                {
                    Page.ShowMessage(WebLanguage.LoadStr(3983, "No results found."));
                    return;
                }
                //// wia...
                if (geosearch.QueryResult[0] == null)
                {
                    Page.ShowMessage(WebLanguage.LoadStr(39831, "No results found (due to internal error)."));
                    return;
                }

                _log.DebugFormat("Zoom to x={0} / y={1} / width={2} / height={3}", geosearch.QueryResult[0].X, geosearch.QueryResult[0].Y, geosearch.QueryResult[0].Width, geosearch.QueryResult[0].Height);

                // use quoted doublevalues to handle german koordinates with ','
                ResultExtent env = geosearch.QueryResult[0];
                ////NumberFormatInfo numberInfo = CultureInfo.CurrentCulture.NumberFormat;
                ////round double values to avoid problems with german cultureinfo
                //string function = string.Format(@"zoomToExtent({0:f0}, {1:f0}, {2:f0}, {3:f0});", env.X, env.Y, env.X + env.Width, env.Y + env.Height);

                string function = string.Format(@"zoomToExtent({0:f0}, {1:f0}, {2:f0}, {3:f0});", env.X, env.Y, env.X + env.Width, env.Y + env.Height);
                ScriptManager.RegisterStartupScript(sender as Control, sender.GetType(), "zoomToExtent", function, true);
            }

            _log.StopTimeMeasure(MsgLevel.Debug, timer, "Total time for searching: ");
        }

        /// <summary>
        /// Sets the display style depending on the search configuration.
        /// Search may be either collapsed or expanded.
        /// </summary>
        private void SetDisplayStyle()
        {
            if (GeoSearchDef.ShowExpanded)
            {
                searchfields.Style.Add("display", "block");
                IMAGENAME = "minimize.png";
            }
            else
            {
                searchfields.Style.Add("display", "none");
                IMAGENAME = "maximize.png";
            }            
        }

        /// <summary>
        /// Adds a script to speed up change of DDLs in IE.
        /// source: http://siderite.blogspot.com/2008/06/very-slow-updatepanel-refresh-when.html
        /// </summary>
        /// <param name="ctrl">The control associated with the script (when in an UpdatePanel)</param>
        private void RegisterScript(Control ctrl)
        {
            string script =
                string.Format(
                    @"
				var select=document.getElementById('{0}'); 
				if (select) {{
					select.parentNode.removeChild(select);
				}}",
                    ctrl.ClientID);
            ScriptManager sm = ScriptManager.GetCurrent(Parent.Page);
            if (sm != null)
            {
                sm.RegisterDispose(ctrl, script);
            }
        }
    }
}