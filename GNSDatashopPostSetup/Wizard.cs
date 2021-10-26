using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs;
using Button = System.Windows.Controls.Button;
using Panel = System.Windows.Controls.Panel;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup
{
    public class Wizard
    {
        #region Fields

        private int m_ActiveIndex;

        #endregion

        #region Properties

        public Button BackButton { get; set; }

        public Button NextButton { get; set; }

        public Button ProcessButton { get; set; }

        public Button CancelButton { get; set; }

        public Panel ContentPanel { get; set; }

        public Expander ProgressExpander { get; set; }

        public ExpanderItem ProgressExpanderItem { get; set; }

        public List<ExpanderItem> ExpanderItems { get; set; }

        public int ActiveIndex
        {
            get
            {
                return m_ActiveIndex;
            }
            private set
            {
                if (value <= 0)
                {
                    m_ActiveIndex = 0;
                    return;
                }
                if (ExpanderItems.Count <= value)
                {
                    m_ActiveIndex = ExpanderItems.Count - 1;
                    return;
                }

                m_ActiveIndex = value;
            }
        }

        public ProgressDialog ProgressDialog { get; set; }

        public RunEvent RunEvent { get; set; }

        public Expander ResultExpander { get; set; }

        public Dispatcher Dispatcher { get; set; }

        #endregion

        #region Constructors

        public Wizard(Button backButton, Button nextButton, Button processButton, Button cancelButton, Panel contentPanel, Expander progressExpander, Expander resultExpander, Dispatcher dispatcher)
        {
            BackButton = backButton;
            backButton.Click += BackButtonClick;

            NextButton = nextButton;
            nextButton.Click += NextButtonClick;

            ProcessButton = processButton;
            ProcessButton.Click += ProcessButtonClick;

            CancelButton = cancelButton;
            CancelButton.Click += CancelButtonClick;

            ContentPanel = contentPanel;

            ProgressExpander = progressExpander;

            ResultExpander = resultExpander;

            Dispatcher = dispatcher;

            ExpanderItems = new List<ExpanderItem>();

            ProgressDialog = new ProgressDialog();
            ProgressExpanderItem = new ExpanderItem(ProgressDialog);
            ProgressExpander.AddExpanderItem(ProgressExpanderItem);

            RunEvent = new RunEvent();
            RunEvent.WriteTextError = ProgressDialog.WriteError;
            RunEvent.WriteTextMessage = ProgressDialog.WriteMessage;
            RunEvent.WriteTextMessageBold = ProgressDialog.WriteBoldMessage;
            RunEvent.WriteNewLine = ProgressDialog.WriteNewLine;
        }

        #endregion

        #region Methods

        public void NextButtonClick(object sender, RoutedEventArgs e)
        {
            NavigateFromActive(1);
        }

        public void BackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigateFromActive(-1);
        }

        public void ProcessButtonClick(object sender, RoutedEventArgs e)
        {
            DisableUsersExpandersAndButtons();
            ActivateProgressDialog();

            MethodInvoker runMethods = InvokeUserDialogsRunMethods;
            runMethods.BeginInvoke(UserDialogsRunMethodsCompleted, null);
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void InvokeUserDialogsRunMethods()
        {
            foreach (ExpanderItem item in ExpanderItems)
            {
                MethodInvoker updateUI = delegate
                        {
                            DeactivateAllUserExpanders();
                            item.Activate();
                        };

                Dispatcher.Invoke(updateUI);
                ProgressDialog.WriteBoldMessage("{0}: ", item.Name);
                try
                {
                    item.Dialog.Run(RunEvent);
                }
                catch (Exception e)
                {
                    ProgressDialog.WriteError("Error occured: " + e.Message);
                }
                SetProgressBar(item.Index + 1);
                ProgressDialog.WriteNewLine();
                Thread.Sleep(700);

            }
        }

        public void SetProgressBar(int currentItemIndex)
        {
            double partPercent = ((double)100) / (ExpanderItems.Count);
            ProgressDialog.ProgressBarValueThreadSafe = partPercent * currentItemIndex;
        }

        private void UserDialogsRunMethodsCompleted(IAsyncResult result)
        {
            MethodInvoker updateUI = delegate
            {
                ResultExpander.AddExpanderItem(ProgressExpanderItem);
                ResultExpander.IsEnabled = true;
                ResultExpander.IsExpanded = true;
                ProgressExpander.IsEnabled = false;
                ProgressExpander.IsExpanded = false;
                ProgressExpanderItem.Name = "Result";
                DeactivateAllUserExpanders();
            };

            Dispatcher.Invoke(updateUI);
        }

        public void AddUserDialog(Expander targetExpander, Dialog dialog)
        {
            ExpanderItem expanderItem = new ExpanderItem(dialog);

            expanderItem.Index = ExpanderItems.Count;
            ExpanderItems.Add(expanderItem);
            targetExpander.AddExpanderItem(expanderItem);
        }

        private void ActivateProgressDialog()
        {

            ProgressExpander.IsEnabled = true;
            ProgressExpanderItem.Activate();
            ShowDialog(ProgressDialog);
        }

        public void ExpanderItemClicked(ExpanderItem item)
        {
            ActivateExpanderItemByIndex(item.Index, false);
        }

        public void NavigateFromActive(int offsetFromActive)
        {
            int followingIndex = ActiveIndex + offsetFromActive;

            ActivateExpanderItemByIndex(followingIndex, false);
        }

        public void ActivateExpanderItemByIndex(int index, bool collapseDeactivatedParentExpander)
        {
            bool isInvalidIndex = (index < 0 || ExpanderItems.Count <= index);
            if (isInvalidIndex)
                return;

            var deactivatedExpItem = ExpanderItems[ActiveIndex];
            deactivatedExpItem.Deactivate(collapseDeactivatedParentExpander);

            ActiveIndex = index;
            var activatedExpItem = ExpanderItems[ActiveIndex];
            activatedExpItem.Activate();

            ShowDialog(activatedExpItem.Dialog);

            BackButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            if (0 < ActiveIndex && ActiveIndex < ExpanderItems.Count)
                BackButton.IsEnabled = true;

            if (0 <= ActiveIndex && ActiveIndex < ExpanderItems.Count - 1)
                NextButton.IsEnabled = true;
        }

        public void DeactivateAllUserExpanders()
        {
            foreach (ExpanderItem item in ExpanderItems)
            {
                item.Deactivate(true);
            }
        }

        public void ShowDialog(Dialog dialog)
        {
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(dialog);
        }

        public void DisableUsersExpandersAndButtons()
        {
            foreach (ExpanderItem item in ExpanderItems)
            {
                item.ParentExpander.IsExpanded = false;
                item.ParentExpander.IsEnabled = false;
            }

            BackButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            ProcessButton.IsEnabled = false;
        }

        #endregion
    }




}
