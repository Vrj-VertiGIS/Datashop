using System;

namespace GEOCOM.GNSDatashop.ServiceContracts
{
    public class TemplateLimitReachedException : Exception
    {
        public TemplateLimitReachedException()
        {
        }

        public TemplateLimitReachedException(string message) : base(message)
        {
        }
    }

}