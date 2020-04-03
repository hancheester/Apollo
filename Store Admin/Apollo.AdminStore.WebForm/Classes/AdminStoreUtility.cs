using Apollo.Core.Domain;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class AdminStoreUtility
    {
        private const string URL_PRODUCT_SPLITTER = "product/";
        private const string URL_CATEGORY_SPLITTER = "category/";
        private const string URL_BRAND_SPLITTER = "brand/";
        private const string URL_BLOG_SPLITTER = "blog/";

        private readonly IShippingService _shippingService;
        private readonly IUtilityService _utilityService;
        private readonly IOrderService _orderService;
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly object obj = new object();

        public AdminStoreUtility(
            IShippingService shippingService,
            IUtilityService utilityService,
            IOrderService orderService,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings)
        {
            _shippingService = shippingService;
            _utilityService = utilityService;
            _orderService = orderService;
            _mediaSettings = mediaSettings;
            _storeInformationSettings = storeInformationSettings;
        }

        public string CleanFtbOutput(string xhtml)
        {
            //xhtml = xhtml.Replace("<div>", string.Empty);
            //xhtml = xhtml.Replace("</div>", string.Empty);
            xhtml = xhtml.Trim();
            return xhtml;
        }

        public string GetShippingImage(int shippingOptionId)
        {
            var shipping = _shippingService.GetShippingOptionById(shippingOptionId);

            if (shipping != null)
            {
                if (shipping.Country.ISO3166Code == "GB")
                {
                    if (shipping.Name.ToLower().Contains("next day"))
                    {
                        return "<i class='fa fa-truck' aria-hidden='true' title='Next Day Delivery'></i>";
                    }
                }
                else
                {
                    return "<i class='fa fa-plane' aria-hidden='true' title='International Delivery'></i>";
                }
            }

            return string.Empty;
        }

        public string GetShippingCountryImage(int countryId)
        {
            if (countryId == 0) return string.Empty;            
            var country = _shippingService.GetCountryById(countryId);
            if (country == null) return string.Empty;
            if (string.IsNullOrEmpty(country.ISO3166Code)) return string.Empty;
            
            return string.Format("<span class='flag-icon flag-icon-{0}' alt='{1}' title='{1}'></span>", country.ISO3166Code.ToLower(), country.Name);            
        }

        public string GetShippingCountryImage(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode)) return string.Empty;
            var country = _shippingService.GetCountryByCountryCode(countryCode);
            if (country == null) return string.Empty;
            
            return string.Format("<span class='flag-icon flag-icon-{0}' alt='{1}' title='{1}'></span>", country.ISO3166Code.ToLower(), country.Name);            
        }
        
        private string[,] _cleanHtmlFilter = new string[,] { {"<b>", string.Empty},
                                                             {"</b>", string.Empty},
                                                             {"<br>", " "},
                                                             {"<br />", " "},
                                                             {"</li>", ", "},
                                                             {"\t", " "},
                                                             {"\r\n", " "},
                                                             {"\n", " "},
                                                             {"\r", " "},
                                                             {Environment.NewLine, " "} };

        public string CleanHtml(string html, int maxLength)
        {
            for (int i = 0; i < _cleanHtmlFilter.GetLength(0); i++)
                html = html.Replace(_cleanHtmlFilter[i, 0], _cleanHtmlFilter[i, 1]);

            html = RegexType.HtmlTag.Replace(html, string.Empty);

            if (html.Length > maxLength)
                html = html.Substring(0, maxLength) + "...";
            return html;
        }

        public string GenerateRandomPasswordGUID(int length)
        {
            string randomGuid = Guid.NewGuid().ToString("N");
            return randomGuid.Substring(0, length);
        }

        public string BuildXmlString(string xmlRootName, string[] values)
        {
            StringBuilder xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0}>", xmlRootName);
            for (int i = 0; i < values.Length; i++)
            {
                xmlString.AppendFormat("<value>{0}</value>", values[i]);
            }
            xmlString.AppendFormat("</{0}>", xmlRootName);

            return xmlString.ToString();
        }

        public DropDownList GenerateDeliveryList(DropDownList ddl)
        {
            ddl.Items.Add(new ListItem(AppConstant.DEFAULT_SELECT, string.Empty));

            ddl.Items.Add("Standard Delivery");
            ddl.Items.Add("First Class Delivery");
            ddl.Items.Add("Tracked Delivery");
            ddl.Items.Add("International Traceable");
            ddl.Items.Add("Next Day Delivery");
            ddl.Items.Add("Next Business Day");
            ddl.Items.Add("Next Day by 10am");
            ddl.Items.Add("Next Day by 2pm");
            ddl.Items.Add("Saturday Delivery");
            ddl.Items.Add("Standard Delivery");

            return ddl;
        }

        public int GetQuarter(DateTime date)
        {
            if (date.Month >= 1 && date.Month <= 3)
                return 1;
            else if (date.Month >= 4 && date.Month <= 6)
                return 2;
            else if (date.Month >= 7 && date.Month <= 9)
                return 3;
            else
                return 4;
        }

        public string GetLabelColour(string orderStatus)
        {
            switch (orderStatus)
            {
                case OrderStatusCode.STOCK_WARNING:
                case OrderStatusCode.SCHEDULED_FOR_CANCEL:
                    return "label-warning";
                case OrderStatusCode.ORDER_PLACED:
                case OrderStatusCode.SHIPPING:
                case OrderStatusCode.INVOICED:
                case OrderStatusCode.PARTIAL_SHIPPING:
                    return "label-success";
                case OrderStatusCode.CANCELLED:
                case OrderStatusCode.DISCARDED:
                    return "label-danger";
                case OrderStatusCode.ON_HOLD:                
                    return "label-primary";
                case OrderStatusCode.PENDING:
                case OrderStatusCode.AWAITING_REPLY:
                case OrderStatusCode.AWAITING_COMPLETION:
                default:
                    return "label-info";
            }            
        }

        public string TruncateString(int maxLength, string input)
        {
            if (input.Length > maxLength)
                input = input.Substring(0, maxLength);

            return input;
        }

        #region Url helper methods

        public string GetProductMediaUrl(string mediaName)
        {
            return _mediaSettings.ProductMediaLocalPath + mediaName;
        }

        public string GetProductUrl(string urlKey)
        {
            return string.Format(AppConstant.URL_FORM1, _storeInformationSettings.StoreFrontLink, URL_PRODUCT_SPLITTER, urlKey);
        }

        public string GetBlogUrl(string urlKey)
        {
            return string.Format(AppConstant.URL_FORM1, _storeInformationSettings.StoreFrontLink, URL_BLOG_SPLITTER, urlKey);
        }

        public string GetBrandUrl(string urlKey)
        {
            return string.Format(AppConstant.URL_FORM1, _storeInformationSettings.StoreFrontLink, URL_BRAND_SPLITTER, urlKey);
        }

        public string GetCategoryUrl(string topUrlKey = null, string secondUrlKey = null, string thirdUrlKey = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_storeInformationSettings.StoreFrontLink).Append(URL_CATEGORY_SPLITTER);

            if (string.IsNullOrEmpty(topUrlKey)) return sb.ToString();
            sb.Append(topUrlKey);

            if (string.IsNullOrEmpty(secondUrlKey)) return sb.ToString();
            sb.Append("/").Append(secondUrlKey);

            if (string.IsNullOrEmpty(thirdUrlKey)) return sb.ToString();
            sb.Append("/").Append(thirdUrlKey);

            return sb.ToString();
        }

        public string GetFriendlyUrlKey(string text)
        {
            StringBuilder sb = new StringBuilder(text.ToLower());

            for (int i = 0; i < sb.Length; i++)
            {
                int charCode = (int)sb[i];
                if (!(charCode > 96 && charCode < 123) && !(charCode > 47 && charCode < 58))
                    sb[i] = '-';
            }

            while (sb.ToString().Contains("--"))
                sb.Replace("--", "-");

            // Remove last "-".
            while (sb.ToString().LastIndexOf('-') == sb.ToString().Length - 1)
                sb.Remove(sb.ToString().LastIndexOf('-'), 1);

            return sb.ToString();
        }

        #endregion

        #region Control methods

        public UserControl LoadUserControl(Page page, string controlPath, params object[] constructorParameters)
        {
            lock (obj)
            {
                var types = new List<Type>();

                if (constructorParameters != null)
                {
                    foreach (object constParam in constructorParameters)
                        types.Add(constParam.GetType());
                }

                if (page == null) page = new Page();

                UserControl control = page.LoadControl(controlPath) as UserControl;
                ConstructorInfo constructor = control.GetType().BaseType.GetConstructor(types.ToArray());

                if (constructor != null)
                    constructor.Invoke(control, constructorParameters);

                return control;
            }
        }

        public Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            for (int i = 0; i < root.Controls.Count; i++)
            {
                Control foundCtl = FindControlRecursive(root.Controls[i], id);

                if (foundCtl != null)
                    return foundCtl;
            }

            return null;
        }

        public Control FindControlRecursive(Page pg, string id)
        {
            for (int i = 0; i < pg.Controls.Count; i++)
            {
                Control ctrl = FindControlRecursive(pg.Controls[i], id);
                if (ctrl != null) return ctrl;
            }

            return null;
        }

        public Control GetPostBackControl(Page page)
        {
            Control control = null;

            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != string.Empty)
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c is Button)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
        }

        #endregion

        #region Price methods

        public string GetFormattedPrice(object price, string currencyCode, CurrencyType type, decimal exchangeRate = 1M, int places = 4)
        {
            return GetFormattedPrice(Convert.ToDecimal(price), currencyCode, type, exchangeRate, places);
        }
     
        public string GetFormattedPrice(decimal price, string currencyCode, CurrencyType type, decimal exchangeRate = 1M, int places = 4)
        {
            string formatted = string.Empty;

            var foundCurrency = _utilityService.GetCurrencyByCurrencyCode(currencyCode);

            if (foundCurrency != null)
            {
                // TODO: We need to improve this.
                if (price >= 0M)
                {
                    switch (type)
                    {
                        case CurrencyType.Code:
                            formatted = string.Format(foundCurrency.CurrencyCode + AppConstant.PRICE_FORMAT, RoundPrice(price * exchangeRate, places));
                            break;
                        case CurrencyType.HtmlEntity:
                            formatted = string.Format(foundCurrency.HtmlEntity + AppConstant.PRICE_FORMAT, RoundPrice(price * exchangeRate, places));
                            break;
                        case CurrencyType.Symbol:
                            formatted = string.Format(foundCurrency.Symbol + AppConstant.PRICE_FORMAT, RoundPrice(price * exchangeRate, places));
                            break;
                        case CurrencyType.None:
                            formatted = RoundPrice(price * exchangeRate, places).ToString();
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case CurrencyType.Code:
                            formatted = "-" + string.Format(foundCurrency.CurrencyCode + AppConstant.PRICE_FORMAT, RoundPrice(price * exchangeRate * -1, places));
                            break;
                        case CurrencyType.HtmlEntity:
                            formatted = "-" + string.Format(foundCurrency.HtmlEntity + AppConstant.PRICE_FORMAT, RoundPrice(price * exchangeRate * -1, places));
                            break;
                        case CurrencyType.Symbol:
                            formatted = "-" + string.Format(foundCurrency.Symbol + AppConstant.PRICE_FORMAT, RoundPrice(price * exchangeRate * -1, places));
                            break;
                        case CurrencyType.None:
                            formatted = RoundPrice(price * exchangeRate * 1, places).ToString();
                            break;
                    }
                }
            }

            return formatted;
        }
        
        public decimal RoundPrice(decimal value, int places = 4)
        {
            return Math.Round(value, places, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}