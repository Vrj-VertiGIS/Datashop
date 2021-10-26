using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GEOCOM.GNS;
using Button = System.Windows.Controls.Button;
using Panel = System.Windows.Controls.Panel;
using TextBox = System.Windows.Controls.TextBox;


namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog03DatashopMapServer
{
    /// <summary>
    /// Interaction logic for Dialog03DatashopMapServer.xaml
    /// </summary>
    public partial class Dialog03DatashopMapServer : Dialog
    {
        public Dialog03DatashopMapServer()
        {
            InitializeComponent();
        }

        public override string Caption
        {
            get
            {
                return "Map Services";
            }
        }

        public string GNSD_ALLPathThreadSafe
        {
            get
            {
                string path = null;
                MethodInvoker invoker = delegate { path = txtBxAll.Text; };
                Dispatcher.Invoke(invoker);
                return path;
            }
        }

        public string GNSD_AVonlyPathThreadSafe
        {
            get
            {
                string path = null;
                MethodInvoker invoker = delegate { path = txtBxAvonly.Text; };
                Dispatcher.Invoke(invoker);
                return path;
            }
        }

        public override void Run(RunEvent runEvent)
        {
            var ServiceInfos = new[] { new { ServiceType = "MapServer", ServiceName = "GNSD_ALL", MxdPath = GNSD_ALLPathThreadSafe },
                            new { ServiceType = "MapServer", ServiceName = "GNSD_AVonly", MxdPath = GNSD_AVonlyPathThreadSafe }};

            foreach (var info in ServiceInfos)
            {
                if(string.IsNullOrEmpty(info.MxdPath))
                {
                    runEvent.WriteTextMessage("Skipping {0} service - no path specified", info.ServiceName);
                    continue;
                }

                try
                {
                    ServerRegistrator.CreateMapService(info.ServiceType, info.ServiceName, info.MxdPath);
                    runEvent.WriteTextMessage("Successfully created {0} service.", info.ServiceName);
                }
                catch (Exception e)
                {

                    runEvent.WriteTextError("Error occured while creating {0} service. Reason: {1}.", info.ServiceName, e.Message); 
                }
            }

        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\arcgisserver\Datashop\mxd\";
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Mxd files (*.mxd)|*.mxd|All files(*.*)|*.*";
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var button = (Button)sender;
                Panel parentPanel = (Panel)button.Parent;
                string txtBxName = (string)button.DataContext;
                TextBox txtBx = (TextBox)parentPanel.FindName(txtBxName);
                txtBx.Text = openFileDialog.FileName;
            }

        }
    }
}