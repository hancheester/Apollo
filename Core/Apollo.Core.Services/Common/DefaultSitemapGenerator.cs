using Apollo.Core.Domain;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess.Interfaces;
using System.IO;
using System.Linq;
using System.Xml;

namespace Apollo.Core.Services.Common
{
    public class DefaultSitemapGenerator : ISitemapGenerator
    {
        #region Constants

        private const string URLSET = "urlset";
        private const string URL = "url";
        private const string LOC = "loc";
        private const string XMLNS = "xmlns";
        private const string XMLNS_VALUE = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private const string NAMESPACE_URI_VALUE = "http://www.w3.org/2000/xmlns/";

        #endregion

        #region Fields
        
        private readonly IRepository<Product> _productRepository;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IBlogService _blogService;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        public DefaultSitemapGenerator(
            IRepository<Product> productRepository,
            IBrandService brandService,
            ICategoryService categoryService,
            IBlogService blogService,
            StoreInformationSettings storeInformationSettings)
        {
            _productRepository = productRepository;
            _brandService = brandService;
            _categoryService = categoryService;
            _blogService = blogService;
            _storeInformationSettings = storeInformationSettings;
        }

        public byte[] BuildSitemap()
        {
            var xmlDoc = new XmlDocument();
            var declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(declaration);

            var urlset = xmlDoc.CreateElement(URLSET, XMLNS_VALUE);
            xmlDoc.AppendChild(urlset);

            var xmlns = xmlDoc.CreateAttribute(XMLNS);
            xmlns.Value = XMLNS_VALUE;
            urlset.Attributes.Append(xmlns);

            #region Brand
            
            var brands = _brandService.GetActiveBrands();

            foreach (var brand in brands)
            {
                var url = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                urlset.AppendChild(url);

                var loc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                loc.InnerText = string.Format("{0}brand/{1}", _storeInformationSettings.StoreFrontLink, brand.UrlRewrite.ToLower());
                url.AppendChild(loc);
            }

            #endregion

            #region Product

            var products = _productRepository.Table
                .Where(p => p.Enabled == true)
                .Select(p => p.UrlRewrite)
                .ToList();

            foreach (var product in products)
            {
                var url = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                urlset.AppendChild(url);

                var loc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                loc.InnerText = string.Format("{0}product/{1}", _storeInformationSettings.StoreFrontLink, product.ToLower());
                url.AppendChild(loc);

                // AMP links
                var ampUrl = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                urlset.AppendChild(ampUrl);

                var ampLoc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                ampLoc.InnerText = string.Format("{0}amp/product/{1}", _storeInformationSettings.StoreFrontLink, product.ToLower());
                ampUrl.AppendChild(ampLoc);

            }

            #endregion

            #region Category
            
            var categories = _categoryService.GetCategoryOverviewModelForMenu();
            foreach (var item in categories)
            {
                var url = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                urlset.AppendChild(url);

                var loc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                loc.InnerText = string.Format("{0}category/{1}", _storeInformationSettings.StoreFrontLink, item.UrlKey.ToLower());
                url.AppendChild(loc);

                if (item.Children.Count > 0)
                {
                    foreach (var child in item.Children)
                    {
                        url = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                        urlset.AppendChild(url);

                        loc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                        loc.InnerText = string.Format("{0}category/{1}/{2}", _storeInformationSettings.StoreFrontLink, item.UrlKey.ToLower(), child.UrlKey.ToLower());
                        url.AppendChild(loc);

                        if (child.Children.Count > 0)
                        {
                            foreach (var grandChild in child.Children)
                            {
                                url = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                                urlset.AppendChild(url);

                                loc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                                loc.InnerText = string.Format("{0}category/{1}/{2}/{3}", _storeInformationSettings.StoreFrontLink, item.UrlKey.ToLower(), child.UrlKey.ToLower(), grandChild.UrlKey.ToLower());
                                url.AppendChild(loc);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Blog

            var blogposts = _blogService.GetAllBlogPosts();

            foreach (var post in blogposts.Items)
            {
                var url = xmlDoc.CreateElement(URL, XMLNS_VALUE);
                urlset.AppendChild(url);

                var loc = xmlDoc.CreateElement(LOC, XMLNS_VALUE);
                loc.InnerText = string.Format("{0}blog/{1}", _storeInformationSettings.StoreFrontLink, post.UrlKey.ToLower());
                url.AppendChild(loc);
            }

            #endregion

            byte[] bytes;
            using (var stream = new MemoryStream())
            using (var xw = XmlWriter.Create(stream))
            {
                xmlDoc.WriteTo(xw);
                xw.Flush();
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}
