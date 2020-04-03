using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apollo.Core.Services.Payment.SagePay.ElementClass
{
    public class t3mresults
    {
        [XmlArray("rule")]
        public List<rule> rule = new List<rule>();
    }
}
