using System;

namespace GEOCOM.GNSD.DBStore
{
    public class InvalidJobStatusException : Exception
    {
        public InvalidJobStatusException(string msg)
            : base(msg)
        {
        }
    }
}
