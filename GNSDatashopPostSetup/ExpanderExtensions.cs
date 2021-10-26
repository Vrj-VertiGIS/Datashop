using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup
{
    public static class ExpanderExtensions
    {
        public static void AddExpanderItem(this Expander expander, ExpanderItem expanderItem)
        {
            var itemList = expander.GetExpanderItemList();
            itemList.Add(expanderItem);
            expanderItem.ParentExpander = expander;
        }

        private static List<ExpanderItem> GetExpanderItemList(this Expander expander)
        {
            ItemsControl items = (ItemsControl)expander.Content;

            if (items.ItemsSource == null)
                items.ItemsSource = new List<ExpanderItem>();

            var itemsList = (List<ExpanderItem>)items.ItemsSource;
            return itemsList;
        }
    }
}
