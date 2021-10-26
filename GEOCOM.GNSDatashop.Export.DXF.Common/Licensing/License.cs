namespace GEOCOM.GNSDatashop.Export.DXF.Common.Licensing
{
    public class License
    {
        public enum LicenseStatus
        {
            untested,       // Not yet tested
            invalid,        // Check-out invalid - license perhaps available/unavailable (this is only the coutcome of the test - not a real status!)
            unavailable,    // License feature unavailable - thus cannot be checked out
            available,      // License feature available - can be checked out
            checkedOut      // License has been successfully checked out
        };

        private LicenseStatus _status = LicenseStatus.untested;

        private static object _semaphore = new object();

        public LicenseStatus Status
        {
            get => _status;
            set
            {
                lock (_semaphore)   // This might be done by several threads in the course of license checking
                {
                    _status = value;
                }
            }
        }

        public string FeatureName { get; set; }
        public string Version { get; set; }

        public License(string featureName, string version)
        {
            FeatureName = featureName;
            Version = version;
            _status = LicenseStatus.untested;
        }

        public override string ToString()
        {
            return string.Format("{0} - Version {1} - Status {2}", FeatureName, Version, _status.ToString());
        }

    }
}
