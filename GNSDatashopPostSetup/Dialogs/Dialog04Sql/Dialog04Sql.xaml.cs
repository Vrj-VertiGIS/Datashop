using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog04Sql
{
    /// <summary>
    /// Interaction logic for Dialog04Sql.xaml
    /// </summary>
    public partial class Dialog04Sql : Dialog
    {
        public Dialog04Sql()
        {
            InitializeComponent();
        }

        public override string Caption
        {
            get
            {
                return "Database";
            }
           
        }

        public override void Run(RunEvent runEvent)
        {
            
        }
    }
}
