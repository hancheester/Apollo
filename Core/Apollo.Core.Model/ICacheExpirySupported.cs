using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model
{
    public interface ICacheExpirySupported
    {
        [DataMember]
        DateTime? CacheExpiryDate { get; set; }
    }
}
