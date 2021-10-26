using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI;
using GEOCOM.GNSDatashop.Export.DXF.Common.Licensing;

using log4net;
using log4net.Config;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command
{
    /// <summary>
    /// Summary description for TestCommand.
    /// </summary>
    /// 
    [ComVisible(true)]
    [Guid(DxfExportCommand.GUID)]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(DxfExportCommand.PROGID)]
    public sealed class DxfExportCommand : BaseCommand
    {
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

        public const string PROGID = "Geocom.ArcMap.DxfExportCommand";
        public const string GUID = "d2ac10de-df76-4349-9074-3936d0a1a601";

        private IApplication _application;
        private IDockableWindow _dockWindow;

        private StoLanguage _lng = new StoLanguage() { AppName = Product.TechnicalAppname };

        private LicenseHolder _licenseHolder = null;

        public DxfExportCommand()
        {
            UpdateCommandButtonCaptions();

            m_bitmap = Images.DxfExportCommandButtonImage; 
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            m_enabled = false;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            var ui = GetUI();
            if (null != ui)
                ui.Show(!ui.IsVisible());
        }

        #endregion

        public override bool Checked => Enabled && GetUI().IsVisible();

        public override bool Enabled
        {
            get
            {
                if (null == _licenseHolder)
                    // Do not initialize this earlier as otherwise we might start it on a dummy instance
                    PrepareLicenseMonitoring();

                _licenseHolder.CheckAvailability();
                UpdateCommandButtonCaptions();

                return (null != GetUI() && m_enabled);
            }
        }

        private IDockableWindow GetUI()
        {
            if (_dockWindow == null)
                _dockWindow = ExportControlForm.GetInstance(_application);
            return _dockWindow;
        }

        private void PrepareLicenseMonitoring()
        {
            _licenseHolder = new LicenseHolder(LicenseHolder.Feature_DXF_Export, "5.00");
            _licenseHolder.OnStatusChanged += LicenseStatusChangedEventEventHandler;
        }

        private void LicenseStatusChangedEventEventHandler(object sender, LicenseStatusChangedEventArgs eventArgs)
        {
            var status = eventArgs.Feature.Status;
            m_enabled = ((status == License.LicenseStatus.available) || (status == License.LicenseStatus.checkedOut));
        }

        private void UpdateCommandButtonCaptions()
        {
            var licStatus = _licenseHolder?.Feature?.Status ?? License.LicenseStatus.unavailable;
            base.m_category = "VertiGIS DXF Tools"; //localizable text
            base.m_caption = _lng.FmtLoadStr(10300, "Dxf export for ArcMap {0}", ArcGISVersion);
            base.m_message = _lng.LoadStr(10304, "Export map layers to AutoCad DXF");
            base.m_toolTip = ((licStatus == License.LicenseStatus.available) || (licStatus == License.LicenseStatus.checkedOut))
                ? _lng.LoadStr(10302, "Export map layers to AutoCad DXF")
                : $"{_lng.LoadStr(10302, "Export map layer to AutoCad DXF")} - {_lng.LoadStr(10704, "no license")}";
            base.m_name = "Dxf_Export";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")
        }

        private string ArcGISVersion
        {
            get
            {
#if ARCGIS_10_1
                return "10.1.0";
#endif
#if (ARCGIS_10_2)
                return "10.2.0";
#endif
#if (ARCGIS_10_3)
                return "10.3.0";
#endif
#if (ARCGIS_10_4)
                return "10.4.0";
#endif
#if (ARCGIS_10_5)
                return "10.5.0";
#endif            
#if (ARCGIS_10_6)
                return "10.6.0";
#endif            
#if (ARCGIS_10_7)
                return "10.7.0";
#endif            
#if (ARCGIS_10_8)
                return "10.8.0";
#endif            
            }
        }
    }
}