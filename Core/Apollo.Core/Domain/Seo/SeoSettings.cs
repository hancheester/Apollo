using Apollo.Core.Configuration;
using System.Collections.Generic;

namespace Apollo.Core.Domain.Seo
{
    public class SeoSettings : ISettings
    {
        public string PageTitleSeparator { get; set; }
        public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }
        public string DefaultTitle { get; set; }
        public string DefaultMetaKeywords { get; set; }
        public string DefaultMetaDescription { get; set; }
        /// <summary>
        /// A value indicating whether product META descriptions will be generated automatically (if not entered)
        /// </summary>
        public bool GenerateProductMetaDescription { get; set; }
        /// <summary>
        /// A value indicating whether we should conver non-wetern chars to western ones
        /// </summary>
        public bool ConvertNonWesternChars { get; set; }
        /// <summary>
        /// A value indicating whether unicode chars are allowed
        /// </summary>
        public bool AllowUnicodeCharsInUrls { get; set; }
        /// <summary>
        /// A value indicating whether canonical URL tags should be used
        /// </summary>
        public bool CanonicalUrlsEnabled { get; set; }
        /// <summary>
        /// WWW requires (with or without WWW)
        /// </summary>
        public WwwRequirement WwwRequirement { get; set; }
        /// <summary>
        /// A value indicating whether JS file bundling and minification is enabled
        /// </summary>
        public bool EnableJsBundling { get; set; }
        /// <summary>
        /// A value indicating whether CSS file bundling and minification is enabled
        /// </summary>
        public bool EnableCssBundling { get; set; }
        /// <summary>
        /// A value indicating whether Twitter META tags should be generated
        /// </summary>
        public bool TwitterMetaTags { get; set; }
        /// <summary>
        /// A value indicating whether Open Graph META tags should be generated
        /// </summary>
        public bool OpenGraphMetaTags { get; set; }
        /// <summary>
        /// Slugs (sename) reserved for some other needs
        /// </summary>
        public List<string> ReservedUrlRecordSlugs { get; set; }
    }
}
