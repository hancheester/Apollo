using Apollo.Core.Services.Payment.SagePay.ElementClass;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Apollo.Core.Services.Payment.SagePay
{
    public class Client
    {
        private string _sagepayReportingAdminApiUrl;

        public Client(string sagepayReportingAdminApiUrl)
        {
            _sagepayReportingAdminApiUrl = sagepayReportingAdminApiUrl;
        }

        public vspaccess SubmitRequest(string xml)
        {
            string data = "XML=" + xml;
            Byte[] bytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sagepayReportingAdminApiUrl);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 60000;
            request.KeepAlive = false;
            request.ContentLength = bytes.Length;

            Stream writer = request.GetRequestStream();
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();

            // Getting response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(vspaccess));
                XmlReaderSettings settings = new XmlReaderSettings();
                //settings.ProhibitDtd = false;
                settings.DtdProcessing = DtdProcessing.Prohibit;

                XmlDocument responseXML = new XmlDocument();
                responseXML.Load(response.GetResponseStream());
                response.Close();

                StringBuilder sb = new StringBuilder(responseXML.OuterXml);
                StringReader sr = new StringReader(sb.ToString());

                return (vspaccess)serializer.Deserialize(sr);
            }

            return new vspaccess();
        }
    }
}
