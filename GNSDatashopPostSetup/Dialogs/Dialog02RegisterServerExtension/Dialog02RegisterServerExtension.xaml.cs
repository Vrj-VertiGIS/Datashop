using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GEOCOM.GNS;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog02RegisterServerExtension
{
    public partial class Dialog02RegisterServerExtension : Dialog
    {
        public Dialog02RegisterServerExtension()
        {
            InitializeComponent();
        }

        public override string Caption
        {
            get { return "Server Extensions"; }
        }

        public bool IsRegisterCheckedThreadSafe
        {
            get
            {
                bool isChecked = false;
                MethodInvoker invoker = delegate { isChecked = chbxRegister.IsChecked ?? false; };
                Dispatcher.Invoke(invoker);
                return isChecked;
            }set
            {
                MethodInvoker invoker = delegate { chbxRegister.IsChecked = value; };
                Dispatcher.Invoke(invoker);
            }
        }

        public override void Run(RunEvent runEvent)
        {
            
            if (IsRegisterCheckedThreadSafe)
            {
                try
                {
                    runEvent.WriteTextMessage("Attempting to register GEONIS server Extensions.");
                    RegisterArcGisServerExtension();
                    runEvent.WriteTextMessage("Successfully to register GEONIS server Extensions.");
                }
                catch (Exception e)
                {
                    runEvent.WriteTextError("Error occured while registering GEONIS server Extensions. Reason: " + e.Message);
                }
              
            }
            else
            {
                runEvent.WriteTextMessage("Skipping the registeration of GEONIS server Extensions.");
            }
        }

        // Strings shown in Arc Catalog and ArcGisServer manager
        internal const string CLSID_SEARCH = "GNSDatashopSearchExtension.ServerObjectExtension";
        internal const string NAME_SEARCH = "GNSDatashopSearchExtension";
        internal const string DISPLAY_NAME_SEARCH = "GEONIS server Datashop SearchExtension";
        internal const string DESCRIPTION_SEARCH = "GEONIS server Datashop SearchExtension";

        private static void RegisterArcGisServerExtension()
        {
            try
            {
                 ServerRegistrator.RegisterServer("register", "MapServer", NAME_SEARCH, DISPLAY_NAME_SEARCH, CLSID_SEARCH, DESCRIPTION_SEARCH);
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering mapserver extension", ex);
            }
        }
    }
}