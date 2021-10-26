using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UserControl=System.Windows.Controls.UserControl;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs
{
    public class Dialog : UserControl
    {
        public virtual string Caption { get; set; }

        public virtual void Run(RunEvent runEvent)
        {

        }

    }

   
}
