using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class BasePage : Page
    {
        #region ========== Filter ==========

        protected const string NEWS_FILTER = "newsfilter";
        protected const string QUERY_FILTER = "queryfilter";
        protected const string SEARCH_TERM_ID_FILTER = "searchtermidfilter";
        protected const string REDIRECT_URL_FILTER = "redirecturlfilter";
        protected const string SHIPPING_COUNTRY_ID_FILTER = "shippingcountryidfilter";
        protected const string CURRENCY_CODE_FILTER = "currencycodefilter";
        protected const string TITLE_FILTER = "titlefilter";
        protected const string FROM_DATE_2_FILTER = "fromdate2filter";
        protected const string TO_DATE_2_FILTER = "todate2filter";
        protected const string PROMO_CODE_FILTER = "promocodefilter";
        protected const string OFFER_RULE_ID_FILTER = "offerruleidfilter";
        protected const string COMMENT_FILTER = "commentfilter";
        protected const string ALIAS_FILTER = "aliasfilter";
        protected const string PRODUCT_REVIEW_ID_FILTER = "productreviewid";
        protected const string TRACKING_REF_FILTER = "trackingreffilter";
        protected const string CARRIER_FILTER = "carrierfilter";
        protected const string CONTACT_NUMBER_FILTER = "contactnumberfilter";
        protected const string DOB_FILTER = "dobfilter";
        protected const string HAS_ITEM_IN_CART_FILTER = "hasitemincartfilter";
        protected const string LAST_NAME_FILTER = "lastnamefilter";        
        protected const string USER_ID_FILTER = "useridfilter";
        protected const string SUBSCRIBER_ID_FILTER = "subscriberidfilter";
        protected const string USERNAME_FILTER = "usernamefilter";
        protected const string ORDER_ID_FILTER = "orderidfilter";
        protected const string EMAIL_FILTER = "emailfilter";
        protected const string GRAND_TOTAL_FILTER = "grandtotalfilter";
        protected const string PRODUCT_QUANITITY_FILTER = "productquantityfilter";
        protected const string PRODUCT_NAME_FILTER = "productnamefilter";
        protected const string PRODUCT_NAME_FILTER_2 = "productnamefilter2";
        protected const string EVENT_LOG_ID_FILTER = "eventlogidfilter";
        protected const string TYPE_FILTER = "typefilter";
        protected const string PRODUCT_SIZE_FILTER = "productsizefilter";
        protected const string FROM_PRODUCT_ID_FILTER = "fromproductidfilter";
        protected const string TO_PRODUCT_ID_FILTER = "toproductidfilter";
        protected const string FROM_ORDER_ID_FILTER = "fromorderidfilter";
        protected const string TO_ORDER_ID_FILTER = "toorderidfilter";
        protected const string FROM_DATE_FILTER = "fromdatefilter";
        protected const string TO_DATE_FILTER = "todatefilter";
        protected const string FROM_ACTIVITY_DATE_FILTER = "fromactivitydatefilter";
        protected const string TO_ACTIVITY_DATE_FILTER = "toactivitydatefilter";
        protected const string FROM_ORDER_PLACED_DATE_FILTER = "fromorderplaceddatefilter";
        protected const string TO_ORDER_PLACED_DATE_FILTER = "toorderplaceddatefilter";
        protected const string FROM_LAST_ACTIVITY_DATE_FILTER = "fromlastactivitydatefilter";
        protected const string TO_LAST_ACTIVITY_DATE_FILTER = "tolastactivitydatefilter";
        protected const string BRAND_ID_FILTER = "brandidfilter";
        protected const string CATEGORY_ID_FILTER = "categoryidfilter";
        protected const string CATEGORY_NAME_FILTER = "categorynamefilter";
        protected const string COLOUR_ID_FILTER = "colouridfilter";
        protected const string COLOUR_VALUE_FILTER = "colourvaluefilter";
        protected const string BRAND_NAME_FILTER = "brandnamefilter";
        protected const string PRODUCT_ID_FILTER = "productidfilter";
        protected const string PRODUCT_ID_FILTER_2 = "productidfilter2";
        protected const string PRODUCT_ID_FILTER_3 = "productidfilter3";
        protected const string PRODUCT_ID_FILTER_4 = "productidfilter4";
        protected const string NAME_FILTER = "namefilter";
        protected const string NAME_FILTER_2 = "namefilter2";
        protected const string NAME_FILTER_3 = "namefilter3";
        protected const string NAME_FILTER_4 = "namefilter4";
        protected const string DESCRIPTION_FILTER = "descriptionfilter";
        protected const string DATE_STAMP_FILTER = "datestampfilter";
        protected const string CHOSEN_FILTER = "chosenfilter";
        protected const string CHOSEN_FILTER_2 = "chosenfilter2";
        protected const string CHOSEN_FILTER_3 = "chosenfilter3";
        protected const string STATUS_FILTER = "statusfilter";
        protected const string HAS_UNIQUE_SALE_FILTER = "hasuniquesalefilter";
        protected const string STATUS_CODE_FILTER = "statuscodefilter";
        protected const string BILL_TO_FILTER = "billtofilter";
        protected const string SHIP_TO_FILTER = "shiptofilter";
        protected const string ORDER_STATUS_FILTER = "orderstatusfilter";
        protected const string POINT_TO_REFUND_FILTER = "pointtorefundfilter";
        protected const string VALUE_TO_REFUND_FILTER = "valuetorefundfilter";
        protected const string FROM_QUANTITY_FILTER = "fromquantityfilter";
        protected const string TO_QUANTITY_FILTER = "toquantityfilter";
        protected const string FROM_PRICE_FILTER = "frompricefilter";
        protected const string TO_PRICE_FILTER = "topricefilter";
        protected const string REVIEW_STATUS_FILTER = "reviewstatusfilter";
        protected const string SHIPPING_NAME_FILTER = "shippingnamefilter";
        protected const string ISSUE_FILTER = "issuefilter";
        protected const string PAYMENT_TYPE_FILTER = "paymenttypefilter";
        protected const string PAYMENT_REF_FILTER = "paymentreffilter";
        protected const string SIZE_FILTER = "sizefilter";
        protected const string PRICE_FILTER = "pricefilter";
        protected const string WEIGHT_FILTER = "weightfilter";
        protected const string STOCK_FILTER = "stockfilter";
        protected const string BARCODE_FILTER = "barcodefilter";        
        protected const string IS_ACTIVE_FILTER = "isactivefilter";
        protected const string DISCONTINUED_FILTER = "discontinuredfilter";
        protected const string SHOW_FEATURED_ONLY = "showfeaturedonly";
        protected const string SHOW_NON_FEATURED_ONLY = "shownonfeaturedonly";
        protected const string LAST_UPDATED_BY = "lastupdatedby";
        protected const string BRANCH_ID_FILTER = "branchidfilter";
        protected const string PRICE_CODE_FILTER = "pricecodefilter";
        protected const string ORDER_ADDRESS_FILTER = "orderaddressfilter";
        protected const string PRODUCT_PRODUCTNAME_FILTER = "productnamefilter";
        protected const string SHIPPING_COUNTRY_FILTER = "shippingcountryfilter";

        protected const string BEST_NAME_FILTER = "bestsellersnamefilter";
        protected const string BEST_LASTSOLDFROM_FILTER = "bestsellerslastsoldfromfilter";
        protected const string BEST_LASTSOLDTO_FILTER = "bestsellerslastsoldtofilter";
        
        protected const string COUNTRY_NAME_FILTER = "countrynamefilter";
        protected const string COUNTRY_ID_FILTER = "countryidfilter";
        protected const string ISO3166_CODE_FILTER = "iso3166codefilter";
       
        #endregion

        #region ========== Misc ==========
        
        protected const string OFFER_RULE_ID = "offerruleid";
        protected const string ORDER_STATUS_CODE = "orderstatuscode";
        protected const string ORDER_ISSUE_CODE = "orderissuecode";
        protected const string ORDER_ADDRESS = "orderaddress";
        protected const string IS_LOADED = "isloaded";
        protected const string IS_CANCELLATION = "iscancellation";
        protected const string DEFAULT_ID = "id";
        protected const string PRODUCT_REVIEW_ID = "productreviewid";
        protected const string REFUND_INFO_ID = "refundinfoid";
        protected const string ORDER_SHIPMENT_ID = "ordershipmentid";
        protected const string PRODUCT_ID = "productid";
        protected const string CATEGORY_ID = "categoryid";
        protected const string BRAND_ID = "brandid";
        protected const string BRAND_CATEGORY_ID = "brandcategoryid";
        protected const string ORDER_ID = "orderid";
        protected const string USER_ID = "userid";
        protected const string NEW_PRICE = "New Price";
        protected const string NEW_IMAGE = "New Image";
        protected const string ADD_BRAND_CATEGORY = "Add Brand Category";
        protected const string ENABLED = "enabled";
        protected const string DISABLED = "disabled";
        protected const string EDIT = "edit";
        protected const string DELETE = "delete";
        protected const string TOGGLE = "toggle";
        protected const string PH_RECORD_FOUND = "phRecordFound";
        protected const string PH_RECORD_NOT_FOUND = "phRecordNotFound";        
        protected const string CHOSEN_PRICES = "chosenprices";
        protected const string NOT_CHOSEN_PRICES = "notchosenprices";
        protected const string CHOSEN_CUSTOM_LABEL_GROUPS = "chosencustomlabelgroups";
        protected const string NOT_CHOSEN_CUSTOM_LABEL_GROUPS = "notchosencustomlabelgroups";
        protected const string CHOSEN_PRODUCTS = "chosenproducts";
        protected const string NOT_CHOSEN_PRODUCTS = "notchosenproducts";
        protected const string CHOSEN_ORDERS = "chosenorders";
        protected const string NOT_CHOSEN_ORDERS = "notchosenorders";
        protected const string CHOSEN_COLOURS = "chosencolours";
        protected const string NOT_CHOSEN_COLOURS = "notchosencolours";
        protected const string CHOSEN_REVIEWS = "chosenreviews";
        protected const string NOT_CHOSEN_REVIEWS = "notchosenreviews";
        protected const string CHANGE_STATUS = "changestatus";
        protected const string CHANGE_DISCONTINUED = "changediscontinued";        
        protected const string YES = "yes";
        protected const string NO = "no";
        protected const string ANY = "any";
        protected const string NEW = "new";
        protected const string DELETED = "deleted";
        protected const string SAVED = "saved";
        protected const string IMAGE = "image";
        protected const string STOCK = "stock";
        protected const string TAG = "tag";
        protected const string LAST_CHOSEN_CATEGORY = "lastchosencategory";
        protected const string CATEGORY_NAME = "categoryname";
        protected const string MODE = "mode";
        protected const string ADD_CATEGORY = "Add Category";
        protected const string BRAND_NAME = "brandname";
        protected const string EDIT_INFO = "EditInfo";
        protected const string BRAND_CATEGORY_NAME = "brandcategoryname";
        protected const string LAST_CHOSEN_BRAND_CATEGORY = "lastchosenbrandcategory";
        protected const string HIDDEN = "hidden";
        protected const string NONE = "none";
        protected const string VISIBLE = "visible";
        protected const string BLOCK = "block";
        protected const string VISIBILITY = "visibility";
        protected const string DISPLAY = "display";
        protected const string CHOSEN_PRODUCTS_FEATURED = "chosenproductsfeatured";
        protected const string NOT_CHOSEN_PRODUCTS_FEATURED = "notchosenproductsfeatured";
        protected const string CHOSEN_BRANDS_FEATURED = "chosenbrandsfeatured";
        protected const string NOT_CHOSEN_BRANDS_FEATURED = "notchosenbrandsfeatured";
        protected const string PRODUCT_NAME = "productname";

        protected const string COMMA = ",";
        protected const string UNDERSCORE = "_";
        
        #endregion

        #region ========== Query ==========

        public int QueryOfferRuleId
        {
            get { return GetIntQuery(OFFER_RULE_ID); }
        }
        public string QueryTempOfferRuleId
        {
            get { return GetStringQuery("tempid"); }
        }
        public int QuerySettingId
        {
            get { return GetIntQuery("settingid"); }
        }
        public int QueryId
        {
            get { return GetIntQuery(DEFAULT_ID); }
        }
        public int QueryProductReviewId
        {
            get { return GetIntQuery(PRODUCT_REVIEW_ID); }
        }
        public int QueryProductId
        {
            get { return GetIntQuery(PRODUCT_ID); }
        }
        public int QueryOrderId
        {
            get { return GetIntQuery(ORDER_ID); }
        }
        public int QueryBrandId
        {
            get { return GetIntQuery(BRAND_ID); }
        }
        public int QueryOrderShipmentId
        {
            get { return GetIntQuery(ORDER_SHIPMENT_ID); }
        }
        public int QueryRefundInfoId
        {
            get { return GetIntQuery(REFUND_INFO_ID); }
        }
        public int QueryUserId
        {
            get { return GetIntQuery(USER_ID); }
        }
        public int QueryAccountId
        {
            get { return GetIntQuery("accountid"); }
        }
        public string QueryIsLoaded
        {
            get { return GetStringQuery(IS_LOADED); }
        }
        public int QueryIsCancellation
        {
            get { return GetIntQuery(IS_CANCELLATION); }
        }
        public string QueryOrderIssueCode
        {
            get { return GetStringQuery(ORDER_ISSUE_CODE); }
        }
        public string QueryOrderStatusCode
        {
            get { return GetStringQuery(ORDER_STATUS_CODE); }
        }        
        public int QueryBranchId
        {
            get { return GetIntQuery(QueryKey.ID); }
        }
        public string QueryAddress
        {
            get { return GetStringQuery(ORDER_ADDRESS); }
        }
        public string QueryProductName
        {
            get { return GetStringQuery(PRODUCT_NAME); }
        }

        #endregion

        #region ========== Query Handler ==========

        private string GetStringQuery(string key)
        {
            if (Request.QueryString[key] != null)
                return Request.QueryString[key];
            else
                return string.Empty;
        }

        private int GetIntQuery(string key)
        {
            if ((Request.QueryString[key] != null)
                        && (RegexType.Integer.Match(Request.QueryString[key]).Success))
                return Convert.ToInt32(Request.QueryString[key]);
            else
                return 0;
        }

        #endregion

        #region ========== View State ==========
  
        protected bool IsFireFoxBrowser
        {
            get
            {
                HttpBrowserCapabilities browser = Request.Browser;

                return browser.Browser.ToLower().Contains("firefox");
            }
        }
        
        protected List<ProductGoogleCustomLabelGroupMappingOverviewModel> ChosenCustomLabelGroups
        {
            get
            {
                if (ViewState[CHOSEN_CUSTOM_LABEL_GROUPS] == null)
                    ViewState[CHOSEN_CUSTOM_LABEL_GROUPS] = new List<ProductGoogleCustomLabelGroupMappingOverviewModel>();

                return (List<ProductGoogleCustomLabelGroupMappingOverviewModel>)ViewState[CHOSEN_CUSTOM_LABEL_GROUPS];
            }
            set { ViewState[CHOSEN_CUSTOM_LABEL_GROUPS] = value; }
        }

        protected List<ProductGoogleCustomLabelGroupMappingOverviewModel> NotChosenCustomLabelGroups
        {
            get
            {
                if (ViewState[NOT_CHOSEN_CUSTOM_LABEL_GROUPS] == null)
                    ViewState[NOT_CHOSEN_CUSTOM_LABEL_GROUPS] = new List<ProductGoogleCustomLabelGroupMappingOverviewModel>();

                return (List<ProductGoogleCustomLabelGroupMappingOverviewModel>)ViewState[NOT_CHOSEN_CUSTOM_LABEL_GROUPS];
            }
            set { ViewState[NOT_CHOSEN_CUSTOM_LABEL_GROUPS] = value; }
        }

        protected List<ProductPriceOverviewModel> ChosenPrices
        {
            get
            {
                if (ViewState[CHOSEN_PRICES] == null)
                    ViewState[CHOSEN_PRICES] = new List<ProductPriceOverviewModel>();

                return (List<ProductPriceOverviewModel>)ViewState[CHOSEN_PRICES];
            }
            set { ViewState[CHOSEN_PRICES] = value; }
        }

        protected List<ProductPriceOverviewModel> NotChosenPrices
        {
            get
            {
                if (ViewState[NOT_CHOSEN_PRICES] == null)
                    ViewState[NOT_CHOSEN_PRICES] = new List<ProductPriceOverviewModel>();

                return (List<ProductPriceOverviewModel>)ViewState[NOT_CHOSEN_PRICES];
            }
            set { ViewState[NOT_CHOSEN_PRICES] = value; }
        }

        protected List<CartItem> ChosenToAddItems
        {
            get
            {
                if (ViewState[CHOSEN_PRODUCTS] == null)
                    ViewState[CHOSEN_PRODUCTS] = new List<CartItem>();

                return (List<CartItem>)ViewState[CHOSEN_PRODUCTS];
            }
            set { ViewState[CHOSEN_PRODUCTS] = value; }
        }

        protected List<CartItem> NotChosenToAddItems
        {
            get
            {
                if (ViewState[NOT_CHOSEN_PRODUCTS] == null)
                    ViewState[NOT_CHOSEN_PRODUCTS] = new List<CartItem>();

                return (List<CartItem>)ViewState[NOT_CHOSEN_PRODUCTS];
            }
            set { ViewState[NOT_CHOSEN_PRODUCTS] = value; }
        }

        protected List<int> ChosenProducts
        {
            get
            {
                if (ViewState[CHOSEN_PRODUCTS] == null)
                    ViewState[CHOSEN_PRODUCTS] = new List<int>();

                return (List<int>)ViewState[CHOSEN_PRODUCTS];
            }
            set { ViewState[CHOSEN_PRODUCTS] = value; }
        }

        protected List<int> NotChosenProducts
        {
            get
            {
                if (ViewState[NOT_CHOSEN_PRODUCTS] == null)
                    ViewState[NOT_CHOSEN_PRODUCTS] = new List<int>();

                return (List<int>)ViewState[NOT_CHOSEN_PRODUCTS];
            }
            set { ViewState[NOT_CHOSEN_PRODUCTS] = value; }
        }

        protected List<int> ChosenProductsFeatured
        {
            get
            {
                if (ViewState[CHOSEN_PRODUCTS_FEATURED] == null)
                    ViewState[CHOSEN_PRODUCTS_FEATURED] = new List<int>();

                return (List<int>)ViewState[CHOSEN_PRODUCTS_FEATURED];
            }
            set { ViewState[CHOSEN_PRODUCTS_FEATURED] = value; }
        }

        protected List<int> NotChosenProductsFeatured
        {
            get
            {
                if (ViewState[NOT_CHOSEN_PRODUCTS_FEATURED] == null)
                    ViewState[NOT_CHOSEN_PRODUCTS_FEATURED] = new List<int>();

                return (List<int>)ViewState[NOT_CHOSEN_PRODUCTS_FEATURED];
            }
            set { ViewState[NOT_CHOSEN_PRODUCTS_FEATURED] = value; }
        }

        protected List<int> ChosenBrandsFeatured
        {
            get
            {
                if (ViewState[CHOSEN_BRANDS_FEATURED] == null)
                    ViewState[CHOSEN_BRANDS_FEATURED] = new List<int>();

                return (List<int>)ViewState[CHOSEN_BRANDS_FEATURED];
            }
            set { ViewState[CHOSEN_BRANDS_FEATURED] = value; }
        }

        protected List<int> NotChosenBrandsFeatured
        {
            get
            {
                if (ViewState[NOT_CHOSEN_BRANDS_FEATURED] == null)
                    ViewState[NOT_CHOSEN_BRANDS_FEATURED] = new List<int>();

                return (List<int>)ViewState[NOT_CHOSEN_BRANDS_FEATURED];
            }
            set { ViewState[NOT_CHOSEN_BRANDS_FEATURED] = value; }
        }

        protected List<int> ChosenOrders
        {
            get
            {
                if (ViewState[CHOSEN_ORDERS] == null)
                    ViewState[CHOSEN_ORDERS] = new List<int>();

                return (List<int>)ViewState[CHOSEN_ORDERS];
            }
            set { ViewState[CHOSEN_ORDERS] = value; }
        }

        protected List<int> NotChosenOrders
        {
            get
            {
                if (ViewState[NOT_CHOSEN_ORDERS] == null)
                    ViewState[NOT_CHOSEN_ORDERS] = new List<int>();

                return (List<int>)ViewState[NOT_CHOSEN_ORDERS];
            }
            set { ViewState[NOT_CHOSEN_ORDERS] = value; }
        }

        protected List<int> ChosenReviews
        {
            get
            {
                if (ViewState[CHOSEN_REVIEWS] == null)
                    ViewState[CHOSEN_REVIEWS] = new List<int>();

                return (List<int>)ViewState[CHOSEN_REVIEWS];
            }
            set { ViewState[CHOSEN_REVIEWS] = value; }
        }

        protected List<int> NotChosenReviews
        {
            get
            {
                if (ViewState[NOT_CHOSEN_REVIEWS] == null)
                    ViewState[NOT_CHOSEN_REVIEWS] = new List<int>();

                return (List<int>)ViewState[NOT_CHOSEN_REVIEWS];
            }
            set { ViewState[NOT_CHOSEN_REVIEWS] = value; }
        }
        
        public bool HasState(string key)
        {
            return ViewState[key] != null && !string.IsNullOrEmpty(ViewState[key].ToString()) ? true : false;
        }

        public string GetStringState(string key)
        {
            switch (key)
            {
                case MODE:
                    return ViewState[key] as string ?? NEW;
                case CHOSEN_FILTER:
                case STATUS_FILTER:
                case HAS_UNIQUE_SALE_FILTER:
                case DISCONTINUED_FILTER:
                case HAS_ITEM_IN_CART_FILTER:
                    return ViewState[key] as string ?? ANY;
                default:
                    return ViewState[key] as string ?? string.Empty;
            }
        }

        public int GetIntState(string key)
        {
            switch (key)
            {
                case CATEGORY_ID:
                case LAST_CHOSEN_CATEGORY:
                    return GetProperValue(key, AppConstant.DEFAULT_CATEGORY);
                //return ViewState[key] == null ? AppConstant.DEFAULT_CATEGORY : Convert.ToInt32(ViewState[key]);
                case BRAND_ID:
                    return GetProperValue(key, -1);
                //return ViewState[key] == null ? -1 : Convert.ToInt32(ViewState[key]);
                case BRAND_CATEGORY_ID:
                case LAST_CHOSEN_BRAND_CATEGORY:
                    return GetProperValue(key, AppConstant.DEFAULT_BRAND_CATEGORY);
                //return ViewState[key] == null ? AppConstant.DEFAULT_BRAND_CATEGORY : Convert.ToInt32(ViewState[key]);
                default:
                    return GetProperValue(key, 0);
                    //return ViewState[key] == null ? 0 : Convert.ToInt32(ViewState[key]);
            }
        }
        
        private decimal GetProperValue(string key, decimal defaultValue)
        {
            decimal result = 0;
            if (ViewState[key] != null)
            {
                if (decimal.TryParse(ViewState[key].ToString(), out result))
                    return result;
            }

            return defaultValue;
        }
        
        private int GetProperValue(string key, int defaultValue)
        {
            int result = 0;
            if (ViewState[key] != null)
            {
                if (Int32.TryParse(ViewState[key].ToString(), out result))
                    return result;
            }

            return defaultValue;
        }

        public void SetState(string key, string value)
        {
            if (value == string.Empty)
                DisposeState(key);
            else
                ViewState[key] = value;
        }

        public void SetState(string key, int value)
        {
            ViewState[key] = value;
        }

        protected void DisposeState(string key)
        {
            ViewState[key] = null;
        }

        #endregion
        
        protected override void OnPreRender(EventArgs e)
        {
            const string BRANCH_PENDING_URL = "/order_line_item_branch_pending_default.aspx?id={0}";
            const string BRANCH_STOCK_ADMISSION_URL = "/order_line_item_stock_admission_branch.aspx";
            
            if(BranchStaffKey.Collection.ContainsKey(User.Identity.Name))
            {
                var branchId = BranchStaffKey.Collection[User.Identity.Name];
                var branchPendingUrl = string.Format(BRANCH_PENDING_URL, branchId);

                if (Request.Url.PathAndQuery != branchPendingUrl && (Request.Url.PathAndQuery != BRANCH_STOCK_ADMISSION_URL))
                {
                    Response.Redirect(branchPendingUrl);
                }                    
            }
            
            base.OnPreRender(e);
        }

        private static string[] aspNetFormElements = new string[]
        {
            "__EVENTTARGET",
            "__EVENTARGUMENT",
            "__VIEWSTATE",
            "__EVENTVALIDATION",
            "__VIEWSTATEENCRYPTED",
        };

        protected override void Render(HtmlTextWriter writer)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

            base.Render(htmlWriter);

            string html = stringWriter.ToString();
            int formStart = html.IndexOf("<form");
            int endForm = -1;

            if (formStart >= 0)
                endForm = html.IndexOf(">", formStart);

            if (endForm >= 0)
            {
                StringBuilder viewStateBuilder = new StringBuilder();

                foreach (string element in aspNetFormElements)
                {
                    int startPoint = html.IndexOf("<input type=\"hidden\" name=\"" + element + "\"");

                    if (startPoint >= 0 && startPoint > endForm)
                    {
                        int endPoint = html.IndexOf("/>", startPoint);

                        if (endPoint >= 0)
                        {
                            endPoint += 2;

                            string viewStateInput = html.Substring(startPoint, endPoint - startPoint);

                            html = html.Remove(startPoint, endPoint - startPoint);

                            viewStateBuilder.Append(viewStateInput).Append("\r\n");
                        }
                    }
                }

                if (viewStateBuilder.Length > 0)
                {
                    viewStateBuilder.Insert(0, "\r\n");
                    html = html.Insert(endForm + 1, viewStateBuilder.ToString());
                }
            }
            writer.Write(html);
        }
    }
}