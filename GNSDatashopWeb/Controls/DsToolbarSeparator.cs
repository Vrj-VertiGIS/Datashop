
namespace GEOCOM.GNSD.Web.Controls
{
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// This class renders a span, regardless of the toolbar's orientation. The css class does care.
    /// </summary>
    public class DsToolbarSeparator : HtmlGenericControl
    {
        /// <summary>
        /// This class renders a span, regardless of the toolbar's orientation. The css class does care.
        /// </summary>
        /// <param name="className">The css class name for this separator</param>
        /// <param name="visible">The element will be rendered with display:none</param>
        public DsToolbarSeparator(string className, bool visible)
        {
            this.TagName = "span";
            this.Attributes.Add("class", className);
            if (visible)
            {
                this.Attributes.Add("_visible", "true");
                this.Style.Add("display", "inline-block"); // do tnor put this in the css file, please... 
            }
            else
            {
                this.Attributes.Add("_visible", "false");
                this.Style.Add("display", "none");
            }
        }

    }
}
