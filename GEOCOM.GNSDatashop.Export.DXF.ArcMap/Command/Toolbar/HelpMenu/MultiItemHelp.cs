using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.SystemUI;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.HelpMenu
{
    [Guid(GUID)]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(PROGID)]
    [ComVisible(true)]
    public class MultiItemHelp : IMultiItem, IMultiItemEx
    {
        public const string PROGID = "GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.MultiItemHelp";
        public const string GUID = "5a83c865-6cbe-46c5-9596-8e642f821dc6";

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
        }

        #endregion

        #endregion

        #region Fields

        private int _bitmap;
        private readonly HelpEntries _entries = new HelpEntries();

        private readonly StoLanguage _lng = new StoLanguage() { AppName = Product.TechnicalAppname };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiItemHelp"/> class.
        /// </summary>
        public MultiItemHelp()
        {
            // Initialize the language component
            _lng.LoadFromResFile();
        }

        #endregion

        #region "IMultiItem Implementations"

        /// <summary>
        /// Gets the caption of the multiItem.
        /// </summary>
        /// <value></value>
        public string Caption
        {
            get { return _lng.LoadStr(11004, "Help documents"); }
        }

        /// <summary>
        /// Gets the help context ID associated with this multiItem.
        /// </summary>
        /// <value></value>
        public int HelpContextID
        {
            get { return default(int); }
        }

        /// <summary>
        /// Gets the name of the help file associated with this multiItem.
        /// </summary>
        /// <value></value>
        public string HelpFile
        {
            get { return default(string); }
        }

        /// <summary>
        /// Gets the status bar message for all items on the multiItem.
        /// </summary>
        /// <value></value>
        public string Message
        {
            get { return default(string); }
        }

        /// <summary>
        /// Gets the name of the multiItem.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get
            {
                return PROGID;
            }
        }

        /// <summary>
        /// Occurs when the item at the specified index is clicked.
        /// </summary>
        /// <param name="index">Index of the clicked item.</param>
        public void OnItemClick(int index)
        {
            var entry = _entries.OfCurrentLanguage.ElementAt(index);

            if (!string.IsNullOrEmpty(entry.OnlineUrl))
                Process.Start(entry.OnlineUrl);
            else if (!string.IsNullOrEmpty(entry.OfflinePath))
                Process.Start(entry.OfflinePath);
        }

        /// <summary>
        /// Occurs when the menu that contains the multiItem is about to be displayed.
        /// </summary>
        /// <param name="Hook"></param>
        /// <returns></returns>
        public int OnPopup(object Hook)
        {
            // return amount of help files found in the help file directory
            return _entries.Count;
        }

        /// <summary>
        /// Get_s the item bitmap.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int get_ItemBitmap(int index)
        {
            if (_bitmap == 0)
            {
                _bitmap = Properties.HelpMenuResources.IconOnlineHelp.GetHbitmap().ToInt32();
            }
            return _bitmap;

        }

        /// <summary>
        /// Get_s the item caption.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string get_ItemCaption(int index)
        {
            return _entries.OfCurrentLanguage.ElementAt(index).Title;
        }

        /// <summary>
        /// Get_s the item checked.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public bool get_ItemChecked(int index)
        {
            // help file items can not be checked
            return false;
        }

        /// <summary>
        /// Get_s the item enabled.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public bool get_ItemEnabled(int index)
        {
            // Enable all items
            return true;
        }

        /// <summary>
        /// Get_s the item message.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string get_ItemMessage(int index)
        {
            return _lng.FmtLoadStr(11006, "Shows online documentation \"{0}\" in the standard web browser",
                _entries.OfCurrentLanguage.ElementAt(index).Title);
        }

        /// <summary>
        /// Get_s the item help file.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string get_ItemHelpFile(int index)
        {
            return default(string);
        }

        /// <summary>
        /// Get_s the item help context ID.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int get_ItemHelpContextID(int index)
        {
            return default(int);
        }

        #endregion

    }
}
