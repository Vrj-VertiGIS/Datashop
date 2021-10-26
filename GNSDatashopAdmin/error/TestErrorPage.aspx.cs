using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.WebPDE
{
    public partial class TestErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnError_Click(object sender, EventArgs e)
        {
            string nullstring = null;
            int length = nullstring.Length;
        }
    }
}
