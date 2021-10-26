using System.Collections;

namespace GEOCOM.GNSD.JobEngineController.ServiceInstaller
{
    /// <summary>
    /// This service installer allows to install a service from different assembly.
    /// </summary>
    public class CustomDllServiceInstaller : System.ServiceProcess.ServiceInstaller
    {
        private readonly string _pathToServiceAssembly;

        /// <summary>
        /// This parameter was reversed engineered (decompiled) from the <see cref="ServiceInstaller"/>
        /// </summary>
        private const string AssemblyPathParam = "assemblypath";

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDllServiceInstaller"/> class.
        /// </summary>
        /// <param name="pathToServiceAssembly">The path to the assembly that contains the service implementation.</param>
        public CustomDllServiceInstaller(string pathToServiceAssembly)
        {
            _pathToServiceAssembly = pathToServiceAssembly;
        }

        public override void Install(IDictionary stateSaver)
        {
            Context.Parameters.Remove(AssemblyPathParam);
            Context.Parameters.Add(AssemblyPathParam, _pathToServiceAssembly);
            base.Install(stateSaver);
        }
    }
}