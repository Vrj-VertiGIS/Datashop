using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web.Core.ServerControls
{
    public interface ILabelAndTextBox
    {
        TextBox TextBox { get; }
        Label Label { get; }
    }
}