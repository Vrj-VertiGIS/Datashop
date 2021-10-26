using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GEOCOM.GNSD.Common.ErrorHandling.Exceptions
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public int LanguageCode { get; set; }
    }
}