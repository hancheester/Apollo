﻿using Apollo.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Apollo.Web.Framework.UI
{
    public class PageHeadBuilder : IPageHeadBuilder
    {
        #region Fields

        private static readonly object s_lock = new object();

        private readonly SeoSettings _seoSettings;
        private readonly List<string> _titleParts;
        private readonly List<string> _metaDescriptionParts;
        private readonly List<string> _metaKeywordParts;
        private readonly Dictionary<ResourceLocation, List<ScriptReferenceMeta>> _scriptParts;
        private readonly Dictionary<ResourceLocation, List<string>> _cssParts;
        private readonly List<string> _canonicalUrlParts;
        private readonly List<string> _prevUrlParts;
        private readonly List<string> _nextUrlParts;
        private readonly List<string> _headCustomParts;

        #endregion

        #region Ctor

        public PageHeadBuilder(SeoSettings seoSettings)
        {
            this._seoSettings = seoSettings;
            this._titleParts = new List<string>();
            this._metaDescriptionParts = new List<string>();
            this._metaKeywordParts = new List<string>();
            this._scriptParts = new Dictionary<ResourceLocation, List<ScriptReferenceMeta>>();
            this._cssParts = new Dictionary<ResourceLocation, List<string>>();
            this._canonicalUrlParts = new List<string>();
            this._prevUrlParts = new List<string>();
            this._nextUrlParts = new List<string>();
            this._headCustomParts = new List<string>();
        }

        #endregion

        #region Utilities

        protected string GetBundleVirtualPath(string prefix, string extension, string[] parts)
        {
            if (parts == null || parts.Length == 0)
                throw new ArgumentException("parts");

            //calculate hash
            var hash = "";
            using (SHA256 sha = new SHA256Managed())
            {
                // string concatenation
                var hashInput = "";
                foreach (var part in parts)
                {
                    hashInput += part;
                    hashInput += ",";
                }

                byte[] input = sha.ComputeHash(Encoding.Unicode.GetBytes(hashInput));
                hash = HttpServerUtility.UrlTokenEncode(input);
            }
            //ensure only valid chars
            //hash = SeoExtensions.GetSeName(hash);

            var sb = new StringBuilder(prefix);
            sb.Append(hash);
            //we used "extension" when we had "runAllManagedModulesForAllRequests" set to "true" in web.config
            //now we disabled it. hence we should not use "extension"
            //sb.Append(extension);
            return sb.ToString();
        }

        protected virtual IItemTransform GetCssTranform()
        {
            return new CssRewriteUrlTransform();
        }

        #endregion

        #region Methods

        public void AddTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Add(part);
        }

        public void AppendTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Insert(0, part);
        }

        public string GenerateTitle(bool addDefaultTitle)
        {
            string result = "";
            var specificTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.AsEnumerable().Reverse().ToArray());
            if (!string.IsNullOrEmpty(specificTitle))
            {
                if (addDefaultTitle)
                {
                    //store name + page title
                    switch (_seoSettings.PageTitleSeoAdjustment)
                    {
                        case PageTitleSeoAdjustment.PagenameAfterStorename:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, _seoSettings.DefaultTitle, specificTitle);
                            }
                            break;
                        case PageTitleSeoAdjustment.StorenameAfterPagename:
                        default:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, specificTitle, _seoSettings.DefaultTitle);
                            }
                            break;

                    }
                }
                else
                {
                    //page title only
                    result = specificTitle;
                }
            }
            else
            {
                //store name only
                result = _seoSettings.DefaultTitle;
            }
            return result;
        }
        
        public void AddMetaDescriptionParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaDescriptionParts.Add(part);
        }

        public void AppendMetaDescriptionParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaDescriptionParts.Insert(0, part);
        }

        public string GenerateMetaDescription()
        {
            var metaDescription = string.Join(", ", _metaDescriptionParts.AsEnumerable().Reverse().ToArray());
            var result = !string.IsNullOrEmpty(metaDescription) ? metaDescription : _seoSettings.DefaultMetaDescription;
            return result;
        }

        public void AddMetaKeywordParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaKeywordParts.Add(part);
        }

        public void AppendMetaKeywordParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaKeywordParts.Insert(0, part);
        }

        public string GenerateMetaKeywords()
        {
            var metaKeyword = string.Join(", ", _metaKeywordParts.AsEnumerable().Reverse().ToArray());
            var result = !string.IsNullOrEmpty(metaKeyword) ? metaKeyword : _seoSettings.DefaultMetaKeywords;
            return result;
        }
        
        public void AddScriptParts(ResourceLocation location, string part, bool excludeFromBundle)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(part))
                return;

            _scriptParts[location].Add(new ScriptReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                Part = part
            });
        }

        public void AppendScriptParts(ResourceLocation location, string part, bool excludeFromBundle)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(part))
                return;

            _scriptParts[location].Insert(0, new ScriptReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                Part = part
            });
        }

        public string GenerateScripts(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null)
        {
            if (!_scriptParts.ContainsKey(location) || _scriptParts[location] == null)
                return "";

            if (_scriptParts.Count == 0)
                return "";

            if (!bundleFiles.HasValue)
            {
                //use setting if no value is specified
                bundleFiles = _seoSettings.EnableJsBundling && BundleTable.EnableOptimizations;
            }
            if (bundleFiles.Value)
            {
                var partsToBundle = _scriptParts[location]
                    .Where(x => !x.ExcludeFromBundle)
                    .Select(x => x.Part)
                    .Distinct()
                    .ToArray();
                var partsToDontBundle = _scriptParts[location]
                    .Where(x => x.ExcludeFromBundle)
                    .Select(x => x.Part)
                    .Distinct()
                    .ToArray();


                var result = new StringBuilder();

                if (partsToBundle.Length > 0)
                {
                    //IMPORTANT: Do not use bundling in web farms or Windows Azure
                    string bundleVirtualPath = GetBundleVirtualPath("~/bundles/scripts/", ".js", partsToBundle);
                    //create bundle
                    lock (s_lock)
                    {
                        var bundleFor = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
                        if (bundleFor == null)
                        {
                            var bundle = new ScriptBundle(bundleVirtualPath);
                            //bundle.Transforms.Clear();

                            //"As is" ordering
                            bundle.Orderer = new AsIsBundleOrderer();
                            //disable file extension replacements. renders scripts which were specified by a developer
                            bundle.EnableFileExtensionReplacements = false;
                            bundle.Include(partsToBundle);
                            BundleTable.Bundles.Add(bundle);
                        }
                    }

                    //parts to bundle
                    result.AppendLine(Scripts.Render(bundleVirtualPath).ToString());
                }

                //parts to do not bundle
                foreach (var path in partsToDontBundle)
                {
                    result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>", urlHelper.Content(path));
                    result.Append(Environment.NewLine);
                }
                return result.ToString();
            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var path in _scriptParts[location].Select(x => x.Part).Distinct())
                {
                    result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>", urlHelper.Content(path));
                    result.Append(Environment.NewLine);
                }
                return result.ToString();
            }
        }
        
        public void AddCssFileParts(ResourceLocation location, string part)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<string>());

            if (string.IsNullOrEmpty(part))
                return;

            _cssParts[location].Add(part);
        }

        public void AppendCssFileParts(ResourceLocation location, string part)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<string>());

            if (string.IsNullOrEmpty(part))
                return;

            _cssParts[location].Insert(0, part);
        }

        public string GenerateCssFiles(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null)
        {
            if (!_cssParts.ContainsKey(location) || _cssParts[location] == null)
                return "";

            //use only distinct rows
            var distinctParts = _cssParts[location].Distinct().ToList();
            if (distinctParts.Count == 0)
                return "";
            if (!bundleFiles.HasValue)
            {
                //use setting if no value is specified
                bundleFiles = _seoSettings.EnableCssBundling && BundleTable.EnableOptimizations;
            }
            if (bundleFiles.Value)
            {
                //bundling is enabled
                var result = new StringBuilder();

                var partsToBundle = distinctParts.ToArray();
                if (partsToBundle.Length > 0)
                {
                    //IMPORTANT: Do not use bundling in web farms or Windows Azure
                    //IMPORTANT: Do not use CSS bundling in virtual categories
                    string bundleVirtualPath = GetBundleVirtualPath("~/bundles/styles/", ".css", partsToBundle);

                    //create bundle
                    lock (s_lock)
                    {
                        var bundleFor = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
                        if (bundleFor == null)
                        {
                            var bundle = new StyleBundle(bundleVirtualPath);
                            //bundle.Transforms.Clear();

                            //"As is" ordering
                            bundle.Orderer = new AsIsBundleOrderer();
                            //disable file extension replacements. renders scripts which were specified by a developer
                            bundle.EnableFileExtensionReplacements = false;
                            foreach (var ptb in partsToBundle)
                            {
                                bundle.Include(ptb, GetCssTranform());
                            }
                            BundleTable.Bundles.Add(bundle);
                        }
                    }

                    //parts to bundle
                    result.AppendLine(Styles.Render(bundleVirtualPath).ToString());
                }

                return result.ToString();
            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var path in distinctParts)
                {
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", urlHelper.Content(path));
                    result.Append(Environment.NewLine);
                }
                return result.ToString();
            }
        }
        
        public void AddCanonicalUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _canonicalUrlParts.Add(part);
        }

        public void AppendCanonicalUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _canonicalUrlParts.Insert(0, part);
        }

        public string GenerateCanonicalUrls()
        {
            var result = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParts)
            {
                result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        public void AddPrevUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _prevUrlParts.Add(part);
        }

        public void AppendPrevUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _prevUrlParts.Insert(0, part);
        }

        public string GeneratePrevUrls()
        {
            var result = new StringBuilder();
            foreach (var prevUrl in _prevUrlParts)
            {
                result.AppendFormat("<link rel=\"prev\" href=\"{0}\" />", prevUrl);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        public void AddNextUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _nextUrlParts.Add(part);
        }

        public void AppendNextUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _nextUrlParts.Insert(0, part);
        }

        public string GenerateNextUrls()
        {
            var result = new StringBuilder();
            foreach (var nextUrl in _nextUrlParts)
            {
                result.AppendFormat("<link rel=\"next\" href=\"{0}\" />", nextUrl);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        public void AddHeadCustomParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _headCustomParts.Add(part);
        }

        public void AppendHeadCustomParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _headCustomParts.Insert(0, part);
        }

        public string GenerateHeadCustom()
        {
            //use only distinct rows
            var distinctParts = _headCustomParts.Distinct().ToList();
            if (distinctParts.Count == 0)
                return "";

            var result = new StringBuilder();
            foreach (var path in distinctParts)
            {
                result.Append(path);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
        
        #endregion

        #region Nested classes

        private class ScriptReferenceMeta
        {
            public bool ExcludeFromBundle { get; set; }
            public string Part { get; set; }
        }

        #endregion
    }
}
