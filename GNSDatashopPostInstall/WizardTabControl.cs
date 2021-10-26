using System.Drawing;
using System.Windows.Forms;

namespace GEOCOM.GNSD.PostInstall
{
    class WizardTabControl : TabControl
    {
        public override Rectangle DisplayRectangle
        {
            get
            {
                // Zur Laufzeit Tabs verbergen
                if (DesignMode)
                    return base.DisplayRectangle;
                return new Rectangle(0, 0, Width, Height);
            }
        }

        protected override void OnSelected(TabControlEventArgs e)
        {
            base.OnSelected(e);
            // Die Randbereiche auf 0 setzen
            e.TabPage.Margin = new Padding(0);
            e.TabPage.Padding = new Padding(0);
        }

        public new bool DesignMode
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }
    }
}