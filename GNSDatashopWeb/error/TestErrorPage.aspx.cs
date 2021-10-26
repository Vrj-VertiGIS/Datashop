using System;

namespace GEOCOM.GNSD.Web.error
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
