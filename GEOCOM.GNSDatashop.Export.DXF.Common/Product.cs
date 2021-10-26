using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using System.Deployment.Application;
using System.Reflection;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class Product
    {
        public static string Name
            => ((AssemblyProductAttribute)(AssemblyInformationalVersionAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute)))).Product;

        public static string Version
            => ApplicationDeployment.IsNetworkDeployed
                              ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                              : Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string BuildDate
            => Assembly.GetExecutingAssembly().GetLinkerTime().ToString("dd.MM.yyyy hh:mm");

        public static string TechnicalAppname
            => "GEOCOM.GNSDatashop.Export.DXF.dll";
    }
}
