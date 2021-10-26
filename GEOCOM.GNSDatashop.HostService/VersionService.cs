using System.Reflection;
using System.ServiceModel;

namespace GEOCOM.GNSDatashop.HostService
{
    /// <summary>
    /// Service contract for the version service
    /// </summary>
    [ServiceContract]
    public interface IVersionService
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetVersion();
    }

    /// <summary>
    /// Simple Endpoint implementation as a test
    /// </summary>
    public class VersionService : IVersionService
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly()
                .GetName()
                .Version.ToString();
        }
    }
}