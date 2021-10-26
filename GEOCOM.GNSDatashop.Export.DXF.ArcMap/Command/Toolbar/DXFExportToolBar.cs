using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using GEOCOM.GEONIS.GNBasicToolbar;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.HelpMenu;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar
{
    /// <summary>
    /// Summary description for DXFExportToolBar.
    /// </summary>
    /// 
    [ComVisible(true)]
    [Guid(GUID)]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(PROGID)]
    public sealed class DXFExportToolBar : BaseToolbar
    {
        public const string PROGID = "GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.DXFExportToolBar";

        public const string GUID = "5191a79c-ab29-48d9-a194-d863fded1d72";

        #region Fields

        private StoLanguage _lng = new StoLanguage() { AppName = Product.TechnicalAppname };

        #endregion

        #region COM Registration Function(s)

        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            GNToolbarCommons.RegisterArcMapPremierToolbar(GUID);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            GNToolbarCommons.UnRegisterArcMapPremierToolbar(GUID);
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

        public DXFExportToolBar()
        {
            AddItem(DxfExportCommand.PROGID);
            BeginGroup(); //Separator

            AddItem(MenuHelp.PROGID);
        }

        public override string Caption
        {
            get
            {
                return _lng.LoadStr(10101, "VertiGIS DXF export tools");
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