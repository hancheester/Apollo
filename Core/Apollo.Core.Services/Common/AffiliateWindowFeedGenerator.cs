using Apollo.Core.Domain;
using Apollo.Core.Domain.Media;
using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
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
    public class AffiliateWindowFeedGenerator : IFeedGenerator
    {
        #region Constants

        private const string UTF_16 = "utf-16";
        private const string UTF_8 = "utf-8";
        private const string AW_PRODUCT_XML = "aw_product_xml";
        private const string TEXT_XML = "text/xml";
        private const string MERCHANT = "merchant";
        private const string PRODUCT = "product";
        private const string PID = "pid";
        private const string UPC = "upc";
        private const string EAN = "ean";
        private const string NAME = "name";
        private const string DESC = "desc";
        private const string PROMO_TEXT = "promotext";
        private const string NO = "0";
        private const string YES = "1";
        private const string WEB_OFFER = "weboffer";
        private const string PRE_ORDER = "preorder";
        private const string IN_STOCK = "instock";
        private const string FOR_SALE = "forsale";
        private const string CURRENCY = "currency";
        private const string THUMB_IMG_URL = "thumburl";
        private const string IMG_URL = "imgurl";
        private const string P_URL = "purl";
        private const string DEL_TIME = "deltime";
        private const string CATEGORY = "category";
        private const string BRAND = "brand";
        private const string PRICE = "price";
        private const string ACTUAL_PRICE = "actualp";
        private const string RRP_PRICE = "rrpp";
        private const string STORE_PRICE = "storep";
        private const string STOCK = "stockquant";
        private const string CONDITION = "condition";
        private const string NEW = "new";
        private const string LAST_UPDATED = "lastupdated";
        private const string GBP = "GBP";
        private const string ROOT = "/";

        private const string ITEM_LINK_FORMAT = "{0}/{1}?metrics=aw";
        private const string IMAGE_LINK_PREFIX = "{0}{1}";
        private const string PRODUCT_IMAGE_LINK = "media/product/{0}";
        private const string PRODUCT_TITLE_FORMAT = "{0} {1}";
        private const string PRICE_FORMAT = "{0:0.00}";

        #endregion

        #region Fields

        private readonly ICategoryService _categoryService;        
        private readonly ILogger _logger;
        private readonly IOrderCalculator _orderCalculator;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public AffiliateWindowFeedGenerator(
            ICategoryService categoryService,
            ILogBuilder logBuilder,
            IOrderCalculator orderCalculator,
            StoreInformationSettings storeInformationSettings,
            MediaSettings mediaSettings)
        {
            _categoryService = categoryService;
            _storeInformationSettings = storeInformationSettings;
            _mediaSettings = mediaSettings;
            _orderCalculator = orderCalculator;

            _logger = logBuilder.CreateLogger(GetType().FullName);
            if (_logger == null) throw new NullReferenceException("Object logger is null as Object logBuilder was failed to create logger.");
        }

        #endregion

        public string BuildFeed(IList<Product> products, string countryCode)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime nowInUTC = DateTime.UtcNow;
            string lastUpdated = "@" + Convert.ToString((nowInUTC - baseTime).Ticks / 10000);

            var xmlDoc = new XmlDocument();
            XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(declaration);

            string dtd = "http://www.affiliatewindow.com/DTD/merchant/datafeedupload.1.3.dtd";
            var docType = xmlDoc.CreateDocumentType(MERCHANT, null, dtd, null);
            xmlDoc.AppendChild(docType);

            var merchant = xmlDoc.CreateElement(MERCHANT);
            xmlDoc.AppendChild(merchant);

            var storeUrl = _storeInformationSettings.StoreFrontLink;
            var noImageFile = _mediaSettings.NoImagePath;

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].ProductMedias.Count > 0 && products[i].Enabled && !products[i].Discontinued)
                {
                    for (int j = 0; j < products[i].ProductPrices.Count; j++)
                    {
                        if (products[i].ProductPrices[j].Enabled && products[i].ProductPrices[j].PriceExclTax > 0M && !(products[i].Discontinued && products[i].ProductPrices[j].Stock <= 0))
                        {
                            var product = xmlDoc.CreateElement(PRODUCT);
                            merchant.AppendChild(product);

                            var preOrder = xmlDoc.CreateAttribute(PRE_ORDER);
                            preOrder.Value = NO;
                            product.Attributes.Append(preOrder);

                            var inStock = xmlDoc.CreateAttribute(IN_STOCK);
                            inStock.Value = YES;
                            product.Attributes.Append(inStock);

                            var forSale = xmlDoc.CreateAttribute(FOR_SALE);
                            forSale.Value = YES;
                            product.Attributes.Append(forSale);

                            var webOffer = xmlDoc.CreateAttribute(WEB_OFFER);
                            webOffer.Value = YES;
                            product.Attributes.Append(webOffer);

                            var pid = xmlDoc.CreateElement(PID);
                            pid.InnerText = products[i].ProductPrices[j].Id.ToString();
                            product.AppendChild(pid);

                            var ean = xmlDoc.CreateElement(EAN);
                            ean.InnerText = products[i].ProductPrices[j].Barcode;
                            product.AppendChild(ean);

                            var name = xmlDoc.CreateElement(NAME);
                            name.InnerText = StripHtml(string.Format(PRODUCT_TITLE_FORMAT, products[i].Name, products[i].ProductPrices[j].Option), 500);
                            product.AppendChild(name);

                            var desc = xmlDoc.CreateElement(DESC);
                            desc.InnerText = StripHtml((products[i].Description != string.Empty ? products[i].Description : products[i].Name), 5000);
                            product.AppendChild(desc);

                            var category = xmlDoc.CreateElement(CATEGORY);
                            var categoriesByProductId = _categoryService.GetCategoriesByProductId(products[i].Id);

                            var foundCategory = categoriesByProductId.Where(x => x.Visible == true).FirstOrDefault();
                            if (foundCategory == null) foundCategory = categoriesByProductId.FirstOrDefault();
                            
                            category.InnerText = foundCategory == null ? "Health & Beauty" : foundCategory.CategoryName;
                            product.AppendChild(category);

                            if (products[i].Brand != null)
                            {
                                var brand = xmlDoc.CreateElement(BRAND);
                                brand.InnerText = products[i].Brand.Name;
                                product.AppendChild(brand);
                            }

                            var pUrl = xmlDoc.CreateElement(P_URL);
                            pUrl.InnerText = string.Format(ITEM_LINK_FORMAT,
                                storeUrl + "product/" + products[i].UrlRewrite.ToLower(),
                                products[i].ProductPrices[j].Id.ToString());
                            product.AppendChild(pUrl);

                            var imgUrl = xmlDoc.CreateElement(IMG_URL);
                            if (products[i].ProductMedias.Count > 0)
                            {
                                imgUrl.InnerText = string.Format(IMAGE_LINK_PREFIX, storeUrl, string.Format(PRODUCT_IMAGE_LINK, products[i].ProductMedias[0].MediaFilename));
                                product.AppendChild(imgUrl);

                                var thumbImgUrl = xmlDoc.CreateElement(THUMB_IMG_URL);
                                thumbImgUrl.InnerText = string.Format(IMAGE_LINK_PREFIX, storeUrl, string.Format(PRODUCT_IMAGE_LINK, products[i].ProductMedias[0].ThumbnailFilename));
                                product.AppendChild(thumbImgUrl);
                            }
                            
                            var delTime = xmlDoc.CreateElement(DEL_TIME);
                            delTime.InnerText = products[i].Delivery.TimeLine;
                            product.AppendChild(delTime);

                            var price = xmlDoc.CreateElement(PRICE);
                            product.AppendChild(price);

                            var actualPrice = xmlDoc.CreateElement(ACTUAL_PRICE);
                            actualPrice.InnerText = string.Format("{0:0.00}", Math.Round(products[i].ProductPrices[j].PriceExclTax, 2, MidpointRounding.AwayFromZero));
                            price.AppendChild(actualPrice);

                            var rrpPrice = xmlDoc.CreateElement(RRP_PRICE);
                            rrpPrice.InnerText = string.Format("{0:0.00}", Math.Round(products[i].ProductPrices[j].PriceExclTax, 2, MidpointRounding.AwayFromZero));
                            price.AppendChild(rrpPrice);

                            if (products[i].ProductPrices[j].OfferRuleId <= 0)
                            {
                                var storePrice = xmlDoc.CreateElement(STORE_PRICE);
                                storePrice.InnerText = string.Format("{0:0.00}", Math.Round(products[i].ProductPrices[j].OfferPriceExclTax, 2, MidpointRounding.AwayFromZero));
                                price.AppendChild(storePrice);
                            }

                            var condition = xmlDoc.CreateElement(CONDITION);
                            condition.InnerText = NEW;
                            product.AppendChild(condition);

                            var stock = xmlDoc.CreateElement(STOCK);
                            stock.InnerText = products[i].ProductPrices[j].Stock.ToString();
                            product.AppendChild(stock);

                            var lastUpdate = xmlDoc.CreateElement(LAST_UPDATED);
                            lastUpdate.InnerText = lastUpdated;
                            product.AppendChild(lastUpdate);
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
    }
}
