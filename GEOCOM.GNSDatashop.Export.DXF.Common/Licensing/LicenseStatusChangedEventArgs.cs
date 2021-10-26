using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Licensing
{
    public class LicenseStatusChangedEventArgs : EventArgs
    {
        private License _feature;

        public License Feature => _feature;

        public LicenseStatusChangedEventArgs(License feature)
            : base()
        {
            _feature = feature;
        }
    }
}
