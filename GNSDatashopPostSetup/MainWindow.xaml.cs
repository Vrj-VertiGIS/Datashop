using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GEOCOM.GNSD.JobEngineController.Config;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog01Introduction;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog02RegisterServerExtension;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog03DatashopMapServer;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs.Dialog04Sql;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Wizard wizard;
        public MainWindow()
        {
            InitializeComponent();

            wizard = new Wizard(backButton, nextButton, processButton, cancelButton, contentPanel, ExpProgress, ExpResult, Dispatcher);

            Dialog introductionDialog = new Dialog01Introduction();
            wizard.AddUserDialog(ExpIntroduction, introductionDialog);

            var registerServerExtensionDialog = new Dialog02RegisterServerExtension();
            wizard.AddUserDialog(ExpCommonSettings, registerServerExtensionDialog);

            var mapservicesDialog = new Dialog03DatashopMapServer();
            wizard.AddUserDialog(ExpCommonSettings, mapservicesDialog);

            var sqlDialog = new Dialog04Sql();
            wizard.AddUserDialog(ExpCommonSettings, sqlDialog);

            wizard.ActivateExpanderItemByIndex(0, false);
        }


        public void RouteCallMouseUpEvent(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock block = (TextBlock)sender;
            ExpanderItem associatedItem = (ExpanderItem)block.DataContext;
            wizard.ExpanderItemClicked(associatedItem);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to close this application?", "Closing", MessageBoxButton.YesNo,
                                                   MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

    }


}
