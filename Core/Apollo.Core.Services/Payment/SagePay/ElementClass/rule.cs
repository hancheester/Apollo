using System.Xml.Serialization;

namespace Apollo.Core.Services.Payment.SagePay.ElementClass
{
    public class rule
    {
        [XmlElement("description")]
        public string description;

        [XmlElement("score")]
        public int score;
    }
}
