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
using GEOCOM.GNSD.JobEngineController.Config;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog01Introduction
{
    /// <summary>
    /// Interaction logic for Dialog01Introduction.xaml
    /// </summary>
    public partial class Dialog01Introduction : Dialog
    {
        public Dialog01Introduction()
        {
            InitializeComponent();
         
        }

        public override string Caption
        {
            get
            {
                return "Introduction";
            }
        }

        public override void Run(RunEvent runEvent)
        {
            runEvent.WriteTextMessage("Preparing installation...");
         
            //    // MessageBox.Show("df");
            //    for (int i = 0; i < 2; i++)
            //    {
            //        runEvent.WriteTextMessage("ahoj " + i);
            //        string text = Text;
            //        runEvent.WriteTextMessage(text);
            //        //  runEvent.WriteTextError("error " + i);
            //        //runEvent.WriteTextMessageBold("ahoj " + i);
            //        //runEvent.WriteNewLine();
            //        Thread.Sleep(1000);
            //    }
            //    throw new Exception("aa");
        }
    }
}