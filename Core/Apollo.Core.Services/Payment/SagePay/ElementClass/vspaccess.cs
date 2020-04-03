using System.Xml.Serialization;

namespace Apollo.Core.Services.Payment.SagePay.ElementClass
{
    public class vspaccess
    {
        [XmlElement("command")]
        public string command;

        [XmlElement("vendor")]
        public string vendor;

        [XmlElement("user")]
        public string user;

        [XmlElement("errorcode")]
        public string errorcode;

        [XmlElement("error")]
        public string error;

        [XmlElement("version")]
        public string version;

        [XmlElement("timestamp")]
        public string timestamp;

        [XmlElement("t3mtxid")]
        public string t3mtxid;

        [XmlElement("t3mresults")]
        public t3mresults t3mresults;

        [XmlElement("vendortxcode")]
        public string vendortxcode;

        [XmlElement("t3mid")]
        public string t3mid;

        [XmlElement("t3mscore")]
        public string t3mscore;

        [XmlElement("t3maction")]
        public string t3maction;

        [XmlElement("clientip")]
        public string clientip;

        [XmlElement("iplocation")]
        public string iplocation;

        [XmlElement("password")]
        public string password;

        [XmlElement("signature")]
        public string signature;

        [XmlElement("vpstxid")]
        public string vpstxid;

        [XmlElement("transactiontype")]
        public string txstateid;
       
        [XmlElement("status")]
        public string status;

        [XmlElement("relatedtransactionid")]
        public string relatedtransactionid;

        [XmlElement("relatedamount")]
        public string relatedamount;

        [XmlElement("description")]
        public string description;

        [XmlElement("amount")]
        public string amount;

        [XmlElement("currency")]
        public string currency;

        [XmlElement("started")]
        public string started;

        [XmlElement("completed")]
        public string completed;

        [XmlElement("securitykey")]
        public string securitykey;

        [XmlElement("giftaid")]
        public string giftaid;

        [XmlElement("paymentsystem")]
        public string paymentsystem;

        [XmlElement("paymentsystemdetails")]
        public string paymentsystemdetails;

        [XmlElement("authprocessor")]
        public string authprocessor;

        [XmlElement("accounttype")]
        public string accounttype;

        [XmlElement("vpsauthcode")]
        public string vpsauthcode;

        [XmlElement("bankauthcode")]
        public string bankauthcode;

        [XmlElement("billingfirstnames")]
        public string billingfirstnames;

        [XmlElement("billingsurname")]
        public string billingsurname;

        [XmlElement("billingaddress")]
        public string billingaddress;

        [XmlElement("billingaddress2")]
        public string billingaddress2;

        [XmlElement("billingcity")]
        public string billingcity;

        [XmlElement("billingpostcode")]
        public string billingpostcode;

        [XmlElement("billingcountry")]
        public string billingcountry;

        [XmlElement("deliveryfirstnames")]
        public string deliveryfirstnames;

        [XmlElement("deliverysurname")]
        public string deliverysurname;

        [XmlElement("deliveryaddress")]
        public string deliveryaddress;

        [XmlElement("deliveryaddress2")]
        public string deliveryaddress2;

        [XmlElement("deliverycity")]
        public string deliverycity;

        [XmlElement("deliverypostcode")]
        public string deliverypostcode;

        [XmlElement("deliverycountry")]
        public string deliverycountry;

        [XmlElement("cardholder")]
        public string cardholder;

        [XmlElement("systemused")]
        public string systemused;

        [XmlElement("vpsprotocol")]
        public string vpsprotocol;

        [XmlElement("refunded")]
        public string refunded;

        [XmlElement("repeated")]
        public string repeated;

        [XmlElement("username")]
        public string username;

        [XmlElement("basket")]
        public string basket;

        [XmlElement("applyavscv2")]
        public string applyavscv2;

        [XmlElement("apply3dsecure")]
        public string apply3dsecure;

        [XmlElement("authattempt")]
        public string authattempt;

        [XmlElement("cv2result")]
        public string cv2result;

        [XmlElement("addressresult")]
        public string addressresult;

        [XmlElement("postcoderesult")]
        public string postcoderesult;

        [XmlElement("threedresult")]
        public string threedresult;

        [XmlElement("emailmessage")]
        public string emailmessage;
        
        [XmlElement("locale")]
        public string locale;
        
        [XmlElement("pprotxid")]
        public string pprotxid;

        [XmlElement("surcharge")]
        public string surcharge;

        [XmlElement("last4digits")]
        public string last4digits;
    }
}
