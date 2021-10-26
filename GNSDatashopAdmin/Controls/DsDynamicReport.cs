using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml.Serialization;

namespace GNSDatashopAdmin.Controls
{


    public abstract class DsDynamicReport : Panel
    {
        protected DsDynamicReportConfig Config;

        protected DsDynamicReportData Data;

        protected DsDynamicReport(DsDynamicReportConfig config, DsDynamicReportData data)
        {
            Config = config;
            Data = data;
            if (Config == null)
                throw new Exception("Missing configuration");
            if (Data == null)
                throw new Exception("Missing configuration");
            LoadIt();
        }

        protected abstract void LoadIt();

        public Control FindControlRecursive(string Id)
        {
            return FindControlRecursive(this, Id);
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control ctl in root.Controls)
            {
                var foundCtl = FindControlRecursive(ctl, id);
                if (foundCtl != null) return foundCtl;
            }
            return null;
        }
    }

    /// <summary>
    /// The panels are rendered one after the other
    /// </summary>
    public class DsDynamicReportInline : DsDynamicReport
    {

        public DsDynamicReportInline( DsDynamicReportConfig config, DsDynamicReportData data)
            : base(config, data)
        {

        }

        protected override void LoadIt()
        {
            if (Config.Panels == null) return;
            this.CssClass = Config.ContainerCssClass; 
            foreach (DsDynamicPanelConfig panelConfig in Config.Panels)
            {
                var panel = new DsDynamicInlinePanel(panelConfig, Data);
                Controls.Add(panel.Head);
                Controls.Add(panel.Body);
            }
        }

    }

    /// <summary>
    ///  The panels are rendered with a tabbed control
    /// </summary>
    public class DsDynamicReportTabbed : DsDynamicReport
    {

        public DsDynamicReportTabbed(DsDynamicReportConfig config, DsDynamicReportData data)
            : base(config, data)
        {
        }

        protected override void LoadIt()
        {
            if (Config.Panels == null) return;
            this.CssClass = Config.ContainerCssClass;
            var tabs = new HtmlGenericControl("div");
            tabs.Attributes.Add("class", Config.HeaderCssClass);
            Controls.Add(tabs);
            var firstTab = true;
            foreach (DsDynamicPanelConfig panelConfig in Config.Panels)
            {
                panelConfig.BodyIsCollapsable = true;
                if (firstTab)
                    panelConfig.BodyIsCollapsed = false;
                else
                    panelConfig.BodyIsCollapsed = true;
                firstTab = false;
                var panel = new DsDynamicTabPanel(panelConfig, Data);
                tabs.Controls.Add(panel.Head);
                Controls.Add(panel.Body);
            }
        }
    }

    public static class DsDynamicReportFactory
    {
        public static DsDynamicReport CreateDynamicReport(DsDynamicReportConfig config, DsDynamicReportData data)
        {
            switch (config.ContainerType)
            {
                case DsDynamicReportContainerType.Tabbed:
                    return new DsDynamicReportTabbed(config, data);
                default:
                    return new DsDynamicReportInline(config, data);
            }
        }
    }

    // this is only a helper type
    public class DsDynamicReportData : Dictionary<string, object>
    {
    }
}
