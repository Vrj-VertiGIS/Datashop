using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs
{
    public class RunEvent
    {
        public bool IsCanceled { get; set; }
        public WriteTextDelegate WriteTextMessage { get; set; }
        public WriteTextDelegate WriteTextMessageBold { get; set; }
        public WriteTextDelegate WriteTextError { get; set; }
        public MethodInvoker WriteNewLine { get; set; }

        public delegate void WriteTextDelegate(string format, params string[] args);


    }
}
