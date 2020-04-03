using System.Text.RegularExpressions;

namespace Apollo.AdminStore.WebForm.Classes
{
    #region Struct area

    public struct ImageHandlerType
    {
        public const string BRAND = "brand";
        public const string COLOUR = "colour";
        public const string MEDIA = "media";
        public const string CATEGORY = "category";
        public const string OFFER = "offer";
        public const string LARGE_BANNER = "largebanner";
        public const string MINI_BANNER = "minibanner";
        public const string MEDIUM_BANNER = "mediumbanner";
        public const string OFFER_BANNER = "offerbanner";
    }
    
    public struct HtmlElement
    {
        public const string BR = "<br/>";
        public const string SPACE = "&nbsp;";
    }

    public struct AppConstant
    {
        public const string ZERO = "0";
        public const string ONE = "1";
        
        public const string XML_ROOT_STATUS = "status";

        public const string PRICE_FORMAT = "{0:0.00}";
        public const string SPACE = " ";
        public const string DATE_FORM1 = "dd/MM/yyyy";
        public const string DATE_FORM2 = "yyyy-MM-dd";
        public const string URL_FORM1 = "{0}{1}{2}";
        
        public const string DEFAULT_SELECT = " - Select - ";        
        
        public const int DEFAULT_BRAND_CATEGORY = -1;
        public const int DEFAULT_CATEGORY = 0;
        public const int DEFAULT_CATEGORY_FILTER = 0;
        public const int DEFAULT_BRAND_FILTER = 0;
        public const int DEFAULT_BRAND = -1;

        public const int SHOW_MAX_COMMENT = 5;

        public const int RED_ALERT_DUE = 5;
        public const int ORANGE_ALERT_DUE = 4;       
    }

    public struct RegexType
    {
        public static readonly Regex Integer = new Regex(@"^[0-9]\d*$", RegexOptions.Compiled);
        public static readonly Regex HtmlTag = new Regex("<(.|\n)+?>", RegexOptions.Compiled);
        public static readonly Regex WhiteSpace = new Regex(@"\s", RegexOptions.Compiled);
    }

    public struct EmailReplacementKeys
    {
        public const string METRIC = "@@METRIC";
        public const string EXPIRY_DATE = "@@EXPIRYDATE";
        public const string EMAIL_NAME = "@@EMAILNAME";
        public const string EMAIL_HTML = "@@EMAILHTML";
        public const string BLOG_HTML = "@@BLOGHTML";
        public const string HREF = "@@HREF";
        public const string IMG_SRC = "@@IMGSRC";
        public const string IMG_ALT = "@@IMGALT";
        public const string PRODUCT = "@@PRODUCT";
        public const string NOW_PRICE = "@@NOWPRICE";
        public const string WAS_PRICE = "@@WASPRICEVALUE";
        public const string WAS_PRICE_INFO = "@@WASPRICEINFO";
        public const string PRODUCT_NAME = "@@PRODUCTNAME";
        public const string BLOG_TITLE = "@@BLOGTITLE";
        public const string BLOG_ARTICLE = "@@BLOGARTICLE";
        public const string BLOG_PERMALINK = "@@BLOGPERMALINK";
        public const string BLOG_CATEGORY_URL = "@@BLOGCATEGORYURL";
        public const string BLOG_CATEGORY_TITLE = "@@BLOGCATEGORYTITLE";
        public const string BLOG_CATEGORIES = "@@BLOGCATEGORIES";
    }

    public struct EmailElementsRegex
    {
        public static readonly Regex Promotion = new Regex(@"(?<name>promoInfo)(?<id>[0-9]*)_", RegexOptions.Compiled);
        public static readonly Regex Banner = new Regex(@"(?<name>banner)(?<id>[0-9]*)_(?<promoInfo>promoInfo[0-9]*)_", RegexOptions.Compiled);
        public static readonly Regex Product = new Regex(@"(?<name>product)(?<id>[0-9]*)_(?<promoInfo>promoInfo[0-9]*)_", RegexOptions.Compiled);
        public static readonly Regex BlogItem = new Regex(@"(?<name>blogItem)(?<id>[0-9]*)_", RegexOptions.Compiled);
    }

    public struct EmailElements
    {
        public const string PROMOTION = "promoInfo";
        public const string BANNER = "banner";
        public const string PRODUCT = "product";
        public const string BLOGITEM = "blogItem";
        public const string NAME = "name";
        public const string ITEM_ID = "id";
        public const string PROD_ID = "prodID";
        public const string METRIC = "metric";
        public const string VALUE = "value";
        public const string URL = "url";
        public const string ALT = "alt";
        public const string TITLE = "title";
        public const string ARTICLE = "art";
        public const string PERMALINK = "perm";
        public const string CATEGORIES = "cats";
        public const string CATEGORIY_URLS = "urls";
        public const string EMAIL_INFO = "emailInfo";
        public const string EMAIL_EXPIRY = "expiry";
        public const string NAV_URL = "navurl";
    }
    
    #endregion

    #region Enum area
    
    public enum SysCheckType
    {
        Name = 0,
        Address = 1,
        PostCode = 2,
        Email = 3
    }
    
    #endregion
    
    
}