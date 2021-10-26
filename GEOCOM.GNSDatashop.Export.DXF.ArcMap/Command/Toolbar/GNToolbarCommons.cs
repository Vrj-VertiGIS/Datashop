using System;
using System.Diagnostics;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using Microsoft.Win32;

namespace GEOCOM.GEONIS.GNBasicToolbar
{
	public static class GNToolbarCommons
	{
		/// <summary>
		/// Checks if the toolbar is visible or not
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="uidIdentifier">The uid identifier.</param>
		/// <returns></returns>
		public static bool GetCheckedState(IApplication application, string uidIdentifier)
		{
			UID uid = new UID() {Value = uidIdentifier};
			ICommandBars commandBars = application.Document.CommandBars;
			ICommandBar toolsBar = (ICommandBar) commandBars.Find(uid, false, true);
			if (toolsBar != null) return toolsBar.IsVisible();
			return false;
		}

		/// <summary>
		/// Shows the toolbar
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="uidIdentifier">The uid identifier.</param>
		public static void Show(IApplication application, string uidIdentifier)
		{
			UID uid = new UID() { Value = uidIdentifier };
			ICommandBars commandBars = application.Document.CommandBars;
			ICommandBar toolsBar = (ICommandBar)commandBars.Find(uid, false, false);
			if (toolsBar != null)
			{
				if (!toolsBar.IsVisible())
					toolsBar.Dock(esriDockFlags.esriDockShow, toolsBar);
				else
					toolsBar.Dock(esriDockFlags.esriDockHide, toolsBar);
			}
		}

		/// <summary>
		/// Registers a toolbar as ArcMap premier toolbar
		/// </summary>
		/// <param name="clsid">The CLSID.</param>
		public static void RegisterArcMapPremierToolbar(string clsid)
		{
            string premierToolbarsKeyPath = string.Empty;
            try
            {
                string[] usersKeys = Registry.Users.GetSubKeyNames();
                foreach (string user in usersKeys)
                {
#if ARCGIS_10_0_0_UP
                    premierToolbarsKeyPath = user + @"\Software\ESRI\Desktop10.0\ArcMap\Settings\PremierToolbars\";
#elif ARCGIS_9_3_1_UP
                    premierToolbarsKeyPath = user + @"\Software\ESRI\ArcMap\Settings\PremierToolbars\";
#else // ArcGis 10.0 is default
                    premierToolbarsKeyPath = user + @"\Software\ESRI\Desktop10.0\ArcMap\Settings\PremierToolbars\";
#endif
                    RegistryKey key = Registry.Users.CreateSubKey(premierToolbarsKeyPath);
                    if (key != null)
                    {
                        key.CreateSubKey(clsid);
                        key.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can not add Premier Toolbar registry entry (" + premierToolbarsKeyPath + "\\" + clsid +
                                "): " + e.Message);
            }
		}

		/// <summary>
		/// Unregisters the toolbar as Arcmap premier toolbar
		/// </summary>
		/// <param name="clsid">The CLSID.</param>
		public static void UnRegisterArcMapPremierToolbar(string clsid)
		{
            string premierToolbarsKeyPath = string.Empty;
            try
            {
                string[] usersKeys = Registry.Users.GetSubKeyNames();
                foreach (string user in usersKeys)
                {
#if ARCGIS_10_0_0_UP
                    premierToolbarsKeyPath = user + @"\Software\ESRI\Desktop10.0\ArcMap\Settings\PremierToolbars\";
#elif ARCGIS_9_3_1_UP
                    premierToolbarsKeyPath = user + @"\Software\ESRI\ArcMap\Settings\PremierToolbars\";
#else // ArcGIS 10.0 is default
                    premierToolbarsKeyPath = user + @"\Software\ESRI\Desktop10.0\ArcMap\Settings\PremierToolbars\";
#endif
                    RegistryKey key = Registry.Users.OpenSubKey(premierToolbarsKeyPath, true);
                    if (key != null)
                    {
                        key.DeleteSubKey(clsid);
                        key.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can not remove Premier Toolbar registry entry (" + premierToolbarsKeyPath + "\\" +
                                clsid + "): " + e.Message);
            }
		}
	}
}
