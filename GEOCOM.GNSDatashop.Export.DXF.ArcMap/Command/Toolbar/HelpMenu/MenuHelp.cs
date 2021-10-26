using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using GEOCOM.GNSDatashop.Export.DXF.Common;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.HelpMenu
{
    /// <summary>
    /// Summary description for MenuHelp.
    /// </summary>
    [Guid(GUID)]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(PROGID)]
    [ComVisible(true)]
    public sealed class MenuHelp : BaseMenu, IRootLevelMenu
    {
        public const string GUID = "29ec8f08-7232-4b6c-ade9-d879df5e0c73";
        public const string PROGID = "GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.MenuHelp";
        #region Fields

        readonly StoLanguage _lng = new StoLanguage() { AppName = Product.TechnicalAppname };

        #endregion

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }

        #endregion
        #endregion

        public MenuHelp()
        {
            AddItem(MultiItemHelp.PROGID);
        }

        public override string Caption
        {
            get
            {
                return _lng.LoadStr(11002, "Help");
            }
        }
        public override string Name
        {
            get
            {
                return PROGID;
            }
        }
    }
}   