using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GNSDatashopAdmin.Controls
{
    /*
     *  This file includes all classes related to the configuration of the DsDynamicReport.
     *  This means : report, panels, tables, rows and cells.
     */

    public delegate string BuildClickCallback(string collapsedCssClass, string expandedCssClass);

    public abstract class DsDynamicPanel
    {
        public DsDynamicHeadPanel Head { get; set; }

        public DsDynamicBodyPanel Body { get; set; }

        protected DsDynamicPanel(
            DsDynamicPanelConfig config,
            DsDynamicReportData data,
            string defaultHeadCssClass,
            string defaultHeadCollapsedCssClass,
            string defaultHeadExpandedCssClass,
            string defaultHeadHoverCssClass,
            string defaultBodyCssClass,
            bool isTabbed)
        {
            if (string.IsNullOrEmpty(config.HeadCssClass))
                config.HeadCssClass = defaultHeadCssClass;
            if (string.IsNullOrEmpty(config.HeadCollapsedCssClass))
                config.HeadCollapsedCssClass = defaultHeadCollapsedCssClass;
            if (string.IsNullOrEmpty(config.HeadExpandedCssClass))
                config.HeadExpandedCssClass = defaultHeadExpandedCssClass;
            if (string.IsNullOrEmpty(config.HeadHoverCssClass)) 
                config.HeadHoverCssClass = defaultHeadHoverCssClass;
            if (string.IsNullOrEmpty(config.BodyCssClass)) 
                config.BodyCssClass = defaultBodyCssClass;
            Body = new DsDynamicBodyPanel(config);
            Head = new DsDynamicHeadPanel(config, Body, BuildClickMethod);
            if (config.Tables != null)
            {
                foreach (DsDynamicTableConfig tableConfig in config.Tables)
                {
                    var table = new DsDynamicTable(tableConfig, data);
                    Body.Controls.Add(table);
                }
            }
        }

        public abstract string BuildClickMethod(string collapsedCssClass, string expandedCssClass);
    }

    public class DsDynamicInlinePanel : DsDynamicPanel
    {
        public DsDynamicInlinePanel(DsDynamicPanelConfig config, DsDynamicReportData data)
            : base(config, data, "DSDRInlineHead", "DSDRInlineCollapsed", "DSDRInlineExpanded", "DSDRInlineHover", "DSDRInlinePanel", false) 
        {
        }

        public override string BuildClickMethod(string collapsedCssClass, string expandedCssClass) 
        {
            // this was for the div version
            // return string.Format("DSDRTogglePanel(event, '{0}','{1}','{2}');", Body.ClientID, collapsedCssClass, expandedCssClass);
            // this is the a version
            return string.Format("DSDRTogglePanel('{0}','{1}','{2}','{3}');", Head.ClientID, Body.ClientID, collapsedCssClass, expandedCssClass);
        }
    }

    public class DsDynamicTabPanel : DsDynamicPanel
    {
        public DsDynamicTabPanel(DsDynamicPanelConfig config, DsDynamicReportData data)
            : base(
                config, data, "DSDRTab", "DSDRTabUnselected", "DSDRTabSelected", "DSDRTabHover", "DSDRTabbedPanel", true)
        {
        }

        public override string BuildClickMethod(string collapsedCssClass, string expandedCssClass)
        {
            // this was for the div version
            // return string.Format("DSDRTabs.Select(event, '{0}','{1}','{2}');", buddyClientID, collapsedCssClass, expandedCssClass);
            // this is the a version
            return string.Format("DSDRTabs.Select('{0}','{1}','{2}','{3}');", Head.ClientID, Body.ClientID, collapsedCssClass, expandedCssClass);
        }
    }

    //////public class DsDynamicHeadPanel : Panel
    //////{
    //////    private DsDynamicBodyPanel buddy;
    //////    private DsDynamicPanelConfig config;
    //////    private BuildClickCallback buildClick; 
    //////    public DsDynamicHeadPanel(DsDynamicPanelConfig config, DsDynamicBodyPanel buddy, BuildClickCallback buildClick)
    //////    {
    //////        this.buddy = buddy;
    //////        this.config = config;
    //////        this.buildClick = buildClick;
    //////        this.CssClass = config.HeadCssClass;
    //////        if (config.BodyIsCollapsable)
    //////            this.CssClass = (config.BodyIsCollapsed) ? config.HeadCollapsedCssClass : config.HeadExpandedCssClass;
    //////        var lit = new Literal();
    //////        lit.Text = HttpUtility.HtmlEncode(config.HeadCaption);
    //////        Controls.Add(lit);
    //////        // continue rendering when control is loaded);
    //////        if (config.BodyIsCollapsable)
    //////            this.PreRender += new EventHandler(DsDynamicHeadPanelPreRender);
    //////    }

    //////    void DsDynamicHeadPanelPreRender(object sender, EventArgs e)
    //////    {
    //////        this.Attributes.Add("_panelId", buddy.ClientID);
    //////        var onclick = buildClick(buddy.ClientID, config.HeadCollapsedCssClass, config.HeadExpandedCssClass);
    //////        if (!string.IsNullOrEmpty(config.OnClientExpand))
    //////            onclick += config.OnClientExpand;
    //////        this.Attributes.Add("onclick", onclick);
    //////    }
    //////}

    public class DsDynamicHeadPanel : HtmlAnchor 
    {
        private readonly DsDynamicBodyPanel buddy;
        private readonly DsDynamicPanelConfig config;
        private readonly BuildClickCallback buildClick;

        public DsDynamicHeadPanel(DsDynamicPanelConfig config, DsDynamicBodyPanel buddy, BuildClickCallback buildClick)
        {
            this.buddy = buddy;
            this.config = config;
            this.buildClick = buildClick;
            Attributes.Add("class", config.HeadCssClass);
            if (config.BodyIsCollapsable)
                Attributes.Add("class", config.BodyIsCollapsed ? config.HeadCollapsedCssClass : config.HeadExpandedCssClass);

            InnerHtml = HttpUtility.HtmlEncode(config.HeadCaption);

            // continue rendering when control is loaded);
            if (config.BodyIsCollapsable)
                PreRender += DsDynamicHeadPanelPreRender;
        }

        private void DsDynamicHeadPanelPreRender(object sender, EventArgs e)
        {
            Attributes.Add("_panelId", buddy.ClientID);
            var onclick = buildClick(config.HeadCollapsedCssClass, config.HeadExpandedCssClass);
            if (!string.IsNullOrEmpty(config.OnClientExpand))
                onclick += config.OnClientExpand;
            
            // Attributes.Add("onclick", onclick);
            HRef = "javascript:" + onclick;
        }
    }

    public class DsDynamicBodyPanel : Panel
    {
        public DsDynamicBodyPanel(DsDynamicPanelConfig config)
        {
            if (!string.IsNullOrEmpty(config.Id))
                this.ID = config.Id;
            this.CssClass = config.BodyCssClass;
            if (config.BodyIsCollapsable)
                if (config.BodyIsCollapsed)
                    this.Style.Add(HtmlTextWriterStyle.Display, "none");
        }
    }
}