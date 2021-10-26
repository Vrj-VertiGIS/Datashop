using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup
{
    public delegate void MouseUpDelegate(ExpanderItem associatedItem);

    public class ExpanderItem : INotifyPropertyChanged
    {
        #region Fields

        private bool m_IsBold;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public bool IsBold
        {
            get
            {
                return m_IsBold;
            }
            set
            {
                m_IsBold = value;
                NotifyPropertyChanged("IsBold");
            }
        }

        public string Name { get; set; }

        public int Index { get; set; }

        public Expander ParentExpander { get; set; }

        public TextBlock TextBlock
        {
            get
            {
                ItemsControl itemsControl = (ItemsControl)ParentExpander.Content;
                DependencyObject dependencyObject = itemsControl.ItemContainerGenerator.ContainerFromItem(this);
                DataTemplate template = itemsControl.ItemTemplate;
                TextBlock textBlock = null;
                if (dependencyObject != null)
                    textBlock = (TextBlock)template.FindName("ExpanderItem", (ContentPresenter)dependencyObject);

                return textBlock;
            }
        }



        public Dialog Dialog { get; set; }
        #endregion

        #region Methods

        public ExpanderItem(Dialog dialog)
        {
            Name = dialog.Caption;
            Dialog = dialog;
            Index = -1;
        }

        public void Activate()
        {
            IsBold = true;
            ParentExpander.IsExpanded = true;
        }

        public void Deactivate(bool collapseParentExpander)
        {
            IsBold = false;
            ParentExpander.IsExpanded = !collapseParentExpander;
        }



        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
