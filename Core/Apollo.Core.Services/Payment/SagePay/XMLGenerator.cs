using Apollo.Core.Services.Payment.SagePay.ElementClass;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Apollo.Core.Services.Payment.SagePay
{
    public class XMLGenerator
    {
        private vspaccess _vspaccess;

        public string command
        {
            set { _vspaccess.command = value; }
        }

        public string vendor
        {
            set { _vspaccess.vendor = value; }
        }

        public string user
        {
            set { _vspaccess.user = value; }
        }

        public string password
        {
            set { _vspaccess.password = value; }
        }

        public string t3mtxid
        {
            set { _vspaccess.t3mtxid = value; }
        }

        public string signature
        {
            set { _vspaccess.signature = value; }
        }

        public string vendortxcode
        {
            set { _vspaccess.vendortxcode = value; }
        }

        public XMLGenerator()
        {
            _vspaccess = new vspaccess();
        }

        public string GetXML()
        {
            string xmlContentOnly = Serialize();

            xmlContentOnly = xmlContentOnly.Replace("<vspaccess>", string.Empty);
            xmlContentOnly = xmlContentOnly.Replace("</vspaccess>", string.Empty);

            string signature = GetSignature(xmlContentOnly);

            _vspaccess.signature = signature;
            _vspaccess.password = null;

            string xml = Serialize();

            return xml;
        }

        private string Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(vspaccess));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.CloseOutput = false;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlWriter xw = XmlWriter.Create(sw, settings);

            serializer.Serialize(xw, _vspaccess, ns);
            sw.Flush();

            return sb.ToString();
        }

        private string GetSignature(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                //to make hex string use lower case instead of uppercase add parameter “X2″
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
