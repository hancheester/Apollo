using Apollo.Core.Domain;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Directory;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Apollo.Core.Services.Common
{
    public class GoogleFeedGenerator : IFeedGenerator
    {
        #region Constants

        private const string RSS = "rss";
        private const string VERSION = "version";
        private const string VERSION_VALUE = "2.0";
        private const string XMLNS = "xmlns";
        private const string G = "g";
        private const string XMLNS_VALUE = "http://base.google.com/ns/1.0";
        private const string NAMESPACE_URI_VALUE = "http://www.w3.org/2000/xmlns/";
        private const string ITEM = "item";
        private const string CHANNEL = "channel";
        private const string TITLE = "title";
        private const string DESCRIPTION = "description";
        private const string LINK = "link";
        private const string LANGUAGE = "language";
        private const string DESCRIPTION_VALUE = "Apollo.co.uk provides great deals on a large range of health and beauty products online. Our products include perfumes, medicines, vitamins, and more.";
        private const string CHANNEL_LINK_VALUE = "{0}?metrics=gprod";        
        private const string ID = "id";
        private const string CONDITION = "condition";
        private const string NEW = "new";
        private const string PRICE = "price";
        private const string SALE_PRICE = "sale_price";
        private const string IMAGE_LINK = "image_link";
        private const string ADDITIONAL_IMAGE_LINK = "additional_image_link";
        private const string BRAND = "brand";
        private const string EAN = "ean";
        private const string GTIN = "gtin";
        private const string IDENTIFIER_EXISTS = "identifier_exists";
        //private const string UPC = "upc";
        private const string IN_STOCK = "in stock";
        private const string OUT_OF_STOCK = "out of stock";
        private const string AVAILABILITY = "availability";
        private const string SHIPPING = "shipping";
        private const string ZERO = "0";
        private const string COUNTRY = "country";
        private const string SHIPPING_WEIGHT = "shipping_weight";
        private const string PRODUCT_TYPE = "product_type";
        private const string GOOGLE_PRODUCT_CATEGORY = "google_product_category";
        private const string WEIGHT_FORMAT = "{0} g";
        private const string HTML_ENTITY_GREATER = " &gt; ";
        private const string DEFAULT_PRODUCT_TYPE_TREE = "Health &amp; Beauty";
        private const string FALSE = "FALSE";

        private const string ITEM_LINK_FORMAT = "{0}/{1}/{2}/{3}?metrics=gprod";

        private const string IMAGE_LINK_PREFIX = "{0}{1}";
        private const string PRODUCT_IMAGE_LINK = "media/product/{0}";
        private const string PRODUCT_TITLE_FORMAT = "{0} {1}";
        private const string N = "n";
        private const string MPN = "mpn";

        private const string USD = "USD";
        private const string AUD = "AUD";
        private const string GBP = "GBP";
        private const string CHF = "CHF";
        private const string INR = "INR";
        private const string EN_US = "en-us";
        private const string EN_GB = "en-gb";
        private const string EN_AU = "en-au";
        private const string GB = "GB";
        private const string AU = "AU";
        private const string US = "US";
        private const string IN = "IN";
        private const string CH = "CH";
        private const string TAX = "tax";
        private const string RATE = "rate";
        private const string TAX_SHIP = "tax_ship";

        private const int MAX_IMAGE_COUNT = 10;

        #endregion

        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IShippingService _shippingService;
        private readonly ICurrencyService _currencyService;
        private readonly IOrderCalculator _orderCalculator;        
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public GoogleFeedGenerator(
            ICategoryService categoryService,
            IShippingService shippingService,
            IOrderCalculator orderCalculator,
            ICurrencyService currencyService,
            StoreInformationSettings storeInformationSettings,
            MediaSettings mediaSettings)
        {
            _categoryService = categoryService;
            _shippingService = shippingService;
            _currencyService = currencyService;
            _storeInformationSettings = storeInformationSettings;
            _mediaSettings = mediaSettings;
            _orderCalculator = orderCalculator;
        }

        #endregion

        public string BuildFeed(IList<Product> products, string countryCode)
        {
            var xmlDoc = new XmlDocument();
            XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(declaration);

            var rss = xmlDoc.CreateElement(RSS);
            xmlDoc.AppendChild(rss);

            var version = xmlDoc.CreateAttribute(VERSION);
            version.Value = VERSION_VALUE;
            rss.Attributes.Append(version);

            var xmlns = xmlDoc.CreateAttribute(XMLNS, G, NAMESPACE_URI_VALUE);
            xmlns.Value = XMLNS_VALUE;
            rss.Attributes.Append(xmlns);

            var channel = xmlDoc.CreateElement(CHANNEL);
            rss.AppendChild(channel);

            var title = xmlDoc.CreateElement(TITLE);
            title.InnerText = _storeInformationSettings.CompanyName;
            channel.AppendChild(title);

            var description = xmlDoc.CreateElement(DESCRIPTION);
            description.InnerText = DESCRIPTION_VALUE;
            channel.AppendChild(description);

            var link = xmlDoc.CreateElement(LINK);
            link.InnerText = string.Format(CHANNEL_LINK_VALUE, _storeInformationSettings.StoreFrontLink);
            channel.AppendChild(link);
            
            var language = xmlDoc.CreateElement(LANGUAGE);
            channel.AppendChild(language);

            var exchangeRate = 1M;
            var currencyCode = GBP;
            var currency = new Currency();
            var targetCountry = _shippingService.GetCountryByCountryCode(countryCode);
            
            switch (countryCode)
            {
                case US:
                    language.InnerText = EN_US;
                    currency = _currencyService.GetCurrencyByCurrencyCode(USD);
                    exchangeRate = currency.ExchangeRate;
                    currencyCode = USD;
                    break;
                case AU:
                    language.InnerText = EN_AU;
                    currency = _currencyService.GetCurrencyByCurrencyCode(AUD);
                    exchangeRate = currency.ExchangeRate;
                    currencyCode = AUD;
                    break;
                case CH:
                    language.InnerText = EN_GB;
                    currency = _currencyService.GetCurrencyByCurrencyCode(CHF);
                    exchangeRate = currency.ExchangeRate;
                    currencyCode = CHF;
                    break;
                case IN:
                    language.InnerText = EN_GB;
                    currency = _currencyService.GetCurrencyByCurrencyCode(INR);
                    exchangeRate = currency.ExchangeRate;
                    currencyCode = INR;
                    break;
                default:
                    language.InnerText = EN_GB;
                    break;
            }

            var storeUrl = _storeInformationSettings.StoreFrontLink;
            var options = _shippingService.GetShippingOptionByCountryAndEnabled(targetCountry.Id, true);
            var noImageFile = _mediaSettings.NoImagePath;

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].ProductMedias.Count > 0)
                {
                    for (int j = 0; j < products[i].ProductPrices.Count; j++)
                    {
                        if (products[i].ProductPrices[j].Enabled 
                            && products[i].ProductPrices[j].PriceExclTax > 0M 
                            && !(products[i].Discontinued && products[i].ProductPrices[j].Stock <= 0))
                        {
                            var item = xmlDoc.CreateElement(ITEM);
                            channel.AppendChild(item);

                            #region ID

                            var id = xmlDoc.CreateElement(G, ID, XMLNS_VALUE);
                            id.InnerText = countryCode + products[i].ProductPrices[j].Id.ToString();
                            item.AppendChild(id);

                            #endregion

                            #region Title

                            var productTitle = xmlDoc.CreateElement(TITLE);
                            productTitle.InnerText = StripHtml(string.Format(PRODUCT_TITLE_FORMAT, products[i].Name, products[i].ProductPrices[j].Option), 150);
                            item.AppendChild(productTitle);

                            #endregion

                            #region Description

                            var productDescription = xmlDoc.CreateElement(DESCRIPTION);
                            productDescription.InnerText = StripHtml((products[i].Description != string.Empty ? products[i].Description : products[i].Name), 1000);
                            item.AppendChild(productDescription);

                            #endregion

                            #region Google Product Category

                            var googleProductCategory = xmlDoc.CreateElement(G, GOOGLE_PRODUCT_CATEGORY, XMLNS_VALUE);
                            googleProductCategory.InnerText = BuildGoogleTaxonomyTree(products[i].GoogleTaxonomyId);
                            item.AppendChild(googleProductCategory);

                            #endregion

                            #region Product Type

                            var productType = xmlDoc.CreateElement(G, PRODUCT_TYPE, XMLNS_VALUE);
                            productType.InnerText = BuildCategoryTree(products[i].Id);
                            item.AppendChild(productType);

                            #endregion
                            
                            #region Link

                            var productLink = xmlDoc.CreateElement(LINK);
                            productLink.InnerText = string.Format(ITEM_LINK_FORMAT,
                                storeUrl + "product/" + products[i].UrlRewrite.ToLower(),
                                products[i].ProductPrices[j].Id.ToString(),
                                countryCode.ToLower(),
                                currencyCode.ToLower());
                            item.AppendChild(productLink);

                            #endregion

                            #region Image

                            var imageLink = xmlDoc.CreateElement(G, IMAGE_LINK, XMLNS_VALUE);
                            imageLink.InnerText = string.Format(IMAGE_LINK_PREFIX, storeUrl, products[i].ProductMedias.Count > 0 ? string.Format(PRODUCT_IMAGE_LINK, products[i].ProductMedias[0].MediaFilename) : noImageFile);
                            item.AppendChild(imageLink);

                            if (products[i].ProductMedias.Count > 1)
                            {
                                for (int k = 1; k < (products[i].ProductMedias.Count % MAX_IMAGE_COUNT); k++)
                                {
                                    var additionalImageLink = xmlDoc.CreateElement(G, ADDITIONAL_IMAGE_LINK, XMLNS_VALUE);
                                    additionalImageLink.InnerText = string.Format(IMAGE_LINK_PREFIX, storeUrl, string.Format(PRODUCT_IMAGE_LINK, products[i].ProductMedias[k].MediaFilename));
                                    item.AppendChild(additionalImageLink);
                                }
                            }

                            #endregion

                            #region Condition

                            var condition = xmlDoc.CreateElement(G, CONDITION, XMLNS_VALUE);
                            condition.InnerText = NEW;
                            item.AppendChild(condition);

                            #endregion
                            
                            #region Stock Availability

                            var availability = xmlDoc.CreateElement(G, AVAILABILITY, XMLNS_VALUE);
                            if ((products[i].ProductPrices[j].Stock <= 0) && (products[i].Discontinued || (products[i].EnforceStockCount || (products[i].Brand != null && products[i].Brand.EnforceStockCount))))
                                availability.InnerText = OUT_OF_STOCK;
                            else
                                availability.InnerText = IN_STOCK;

                            item.AppendChild(availability);

                            #endregion

                            #region Price

                            var price = xmlDoc.CreateElement(G, PRICE, XMLNS_VALUE);
                            price.InnerText = string.Format("{0:0.00}", 
                                Math.Round((targetCountry.IsEC ? products[i].ProductPrices[j].PriceInclTax : products[i].ProductPrices[j].PriceExclTax) * exchangeRate, 2, MidpointRounding.AwayFromZero));
                            item.AppendChild(price);

                            if (products[i].ProductPrices[j].OfferRuleId > 0)
                            {
                                var salePrice = xmlDoc.CreateElement(G, SALE_PRICE, XMLNS_VALUE);
                                salePrice.InnerText = string.Format("{0:0.00}", Math.Round((targetCountry.IsEC ? products[i].ProductPrices[j].OfferPriceInclTax : products[i].ProductPrices[j].OfferPriceExclTax) * exchangeRate, 2, MidpointRounding.AwayFromZero));
                                item.AppendChild(salePrice);
                            }
                            
                            #endregion

                            #region Barcode

                            //// Default is EAN
                            //var ean = xmlDoc.CreateElement(G, EAN, XMLNS_VALUE);
                            //ean.InnerText = GetCompleteBarcode(products[i].ProductPrices[j].Barcode, 13);
                            //item.AppendChild(ean);

                            if (string.IsNullOrEmpty(products[i].ProductPrices[j].Barcode) && string.IsNullOrEmpty(products[i].ProductPrices[j].PriceCode))
                            {
                                var gidentifier_exists = xmlDoc.CreateElement(G, IDENTIFIER_EXISTS, XMLNS_VALUE);
                                gidentifier_exists.InnerText = FALSE;
                                item.AppendChild(gidentifier_exists);
                            }
                            else if (string.IsNullOrEmpty(products[i].ProductPrices[j].Barcode) == false)
                            {
                                var gtin = xmlDoc.CreateElement(G, GTIN, XMLNS_VALUE);

                                if (products[i].ProductPrices[j].Barcode.Length == 12)
                                    gtin.InnerText = products[i].ProductPrices[j].Barcode;
                                else
                                    gtin.InnerText = GetCompleteBarcode(products[i].ProductPrices[j].Barcode, 13);

                                item.AppendChild(gtin);
                            }

                            #endregion

                            #region MPN

                            if (string.IsNullOrEmpty(products[i].ProductPrices[j].PriceCode) == false)
                            {
                                var mpn = xmlDoc.CreateElement(G, MPN, XMLNS_VALUE);
                                mpn.InnerText = products[i].ProductPrices[j].PriceCode;
                                item.AppendChild(mpn);
                            }

                            #endregion

                            #region Brand

                            if (products[i].Brand != null && string.IsNullOrEmpty(products[i].Brand.Name) == false)
                            {
                                var brand = xmlDoc.CreateElement(G, BRAND, XMLNS_VALUE);
                                brand.InnerText = products[i].Brand.Name;
                                item.AppendChild(brand);
                            }

                            #endregion
                            
                            #region Tax

                            switch (countryCode)
                            {
                                case US:
                                    // Tax
                                    var tax = xmlDoc.CreateElement(G, TAX, XMLNS_VALUE);
                                    item.AppendChild(tax);

                                    var taxCountry = xmlDoc.CreateElement(G, COUNTRY, XMLNS_VALUE);
                                    taxCountry.InnerText = countryCode;
                                    tax.AppendChild(taxCountry);

                                    var rate = xmlDoc.CreateElement(G, RATE, XMLNS_VALUE);
                                    rate.InnerText = ZERO;
                                    tax.AppendChild(rate);

                                    var taxShip = xmlDoc.CreateElement(G, TAX_SHIP, XMLNS_VALUE);
                                    taxShip.InnerText = N;
                                    tax.AppendChild(taxShip);
                                    break;
                                default:
                                    break;
                            }

                            #endregion

                            #region Shipping

                            var shipping = xmlDoc.CreateElement(G, SHIPPING, XMLNS_VALUE);
                            item.AppendChild(shipping);

                            var country = xmlDoc.CreateElement(G, COUNTRY, XMLNS_VALUE);
                            country.InnerText = countryCode;
                            shipping.AppendChild(country);

                            var shippingCost = xmlDoc.CreateElement(G, PRICE, XMLNS_VALUE);
                            var cost = _orderCalculator.GetShippingCost(products[i].ProductPrices[j], options[0], 0, 0);
                            shippingCost.InnerText = string.Format("{0:0.00}", Math.Round(cost * exchangeRate, 2, MidpointRounding.AwayFromZero));
                            shipping.AppendChild(shippingCost);

                            #endregion

                            #region Shipping Weight

                            var shippingWeight = xmlDoc.CreateElement(G, SHIPPING_WEIGHT, XMLNS_VALUE);
                            shippingWeight.InnerText = string.Format(WEIGHT_FORMAT, products[i].ProductPrices[j].Weight.ToString());
                            item.AppendChild(shippingWeight);

                            #endregion

                            //TODO: We should prepare custom labels, refer https://support.google.com/merchants/answer/188494?hl=en-GB&ref_topic=3404778#example_labels
                        }
                    }
                }
            }

            using (var sw = new StringWriter())
            using (var xw = XmlWriter.Create(sw))
            {
                xmlDoc.WriteTo(xw);
                xw.Flush();
                return sw.GetStringBuilder().ToString();
            }
        }

        private string BuildCategoryTree(int productId)
        {
            var category = _categoryService.GetFirstActiveCategoryByProductId(productId);
            if (category == null) return DEFAULT_PRODUCT_TYPE_TREE;

            var tree = _categoryService.GetTreeListWithName(category.Id);
            if (tree == null || tree.Count == 0) return DEFAULT_PRODUCT_TYPE_TREE;

            return string.Join(HTML_ENTITY_GREATER, tree.Select(x => x.Value).ToArray());
        }

        private string BuildGoogleTaxonomyTree(int googleTaxonomyId)
        {
            var tree = _categoryService.GetGoogleTaxonomyTreeListWithName(googleTaxonomyId);
            if (tree == null || tree.Count == 0) return DEFAULT_PRODUCT_TYPE_TREE;
            return (tree.Count > 0) ? string.Join(HTML_ENTITY_GREATER, tree.Select(x => x.Value).ToArray()) : DEFAULT_PRODUCT_TYPE_TREE;
        }

        private string[,] _stripHtmlFilter = new string[,] { {"<b>", " "},
                                                                    {"</b>", "; "},
                                                                    {"<br>", " "},
                                                                    {"<br />", " "},
                                                                    {"</li>", ", "},
                                                                    {"’", string.Empty},
                                                                    {"‘", string.Empty},
                                                                    {"…", "."},
                                                                    {"“", string.Empty},
                                                                    {"”", string.Empty},
                                                                    {"™", string.Empty},
                                                                    {"—", " "},
                                                                    {"–", " "},
                                                                    {"μ", "u"},
                                                                    {"\t", " "},
                                                                    {"\r\n", " "},
                                                                    {"\n", " "},
                                                                    {"\r", " "},
                                                                    {Environment.NewLine, " "} };

        private string StripHtml(string html, int maxLength)
        {
            for (int i = 0; i < _stripHtmlFilter.GetLength(0); i++)
                html = html.Replace(_stripHtmlFilter[i, 0], _stripHtmlFilter[i, 1]);

            Regex HtmlTag = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);

            html = HtmlTag.Replace(html, string.Empty);

            if (html.Length > maxLength)
            {
                html = html.Substring(0, maxLength);
                html = html + "...";
            }

            html = RemoveDiacritics(html); // Remove Accented Characters

            return HttpUtility.HtmlEncode(html);
        }

        private string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        private string GetCompleteBarcode(string barcode, int minLength)
        {
            if (barcode.Length < minLength)
            {
                while (barcode.Length < minLength)
                {
                    barcode = ZERO + barcode;
                }
            }

            return barcode;
        }
    }
}
