using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GNSDatashopAdmin.Helpers
{
    using System.Web.UI.WebControls;

    using GNSDatashopAdmin.Config;

    public static class AspNet
    {
        public static Control FindControlRecursive(Control Root, string Id)
        {
            if (Root.ID == Id) return Root;
            foreach (Control ctl in Root.Controls)
            {
                var foundCtl = FindControlRecursive(ctl, Id);
                if (foundCtl != null) return foundCtl;
            }
            return null;
        }

        public static void SetupColumnsFromConfig(JobList jobList, GridView grid)
        {
            if (jobList == null)
            {
                return;
            }

            foreach (DataControlField column in grid.Columns)
            {
                string field = column.SortExpression;                
                var result = jobList.Fields.Where(s => s.Name.Equals(field) && s.Visible);
                if (result.Count() == 0)
                {
                    column.Visible = false;
                }
            }
        }
    }
}
