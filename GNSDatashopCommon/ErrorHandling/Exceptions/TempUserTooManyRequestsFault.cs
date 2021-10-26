using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GEOCOM.GNSD.Common.ErrorHandling.Exceptions
{
    [DataContract]
    public class TempUserTooManyRequestsFault
    {
        [DataMember]
        public int Period { get; set; }

        [DataMember]
        public int Limit { get; set; }
    }
}
