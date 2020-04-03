namespace Apollo.Core.Caching
{
    public struct CacheKey
    {
        public const string CURRENCY_ALL_KEY = "currency.all";
        public const string CURRENCY_BY_ID_KEY = "currency.id-{0}";
        public const string CURRENCY_BY_CURRENCY_CODE_KEY = "currency.currency-code-{0}";
        public const string CURRENCY_COUNTRY_BY_CURRENCY_ID_KEY = "currencycountry.currency-id-{0}";
        public const string CURRENCY_MODEL_LIST_KEY = "currency.currency-model-list";
        public const string CURRENCY_PATTERN_KEY = "currency.";
        public const string CURRENCY_COUNTRY_PATTERN_KEY = "currencycountry.";

        public const string TESTIMONIAL_BY_PRIORITY_KEY = "testimonial.bypriority";
        public const string TESTIMONIAL_PATTERN_KEY = "testimonial.";

        public const string GENERIC_ATTRIBUTE_BY_ENTITY_ID_AND_KEY_GROUP_KEY = "genericattribute.entity-id-{0}.key-group-{1}";
        public const string GENERIC_ATTRIBUTE_PATTERN_KEY = "genericattribute.";

        public const string ACCOUNT_BY_ID_KEY = "account.id-{0}";
        public const string ACCOUNT_BY_USERNAME_KEY = "account.username-{0}";
        public const string ACCOUNT_ROLES_BY_USERNAME_KEY = "account.roles.username-{0}";
        public const string ACCOUNT_PATTERN_KEY = "account.";

        public const string PROFILE_ID_BY_USERNAME_KEY = "profile.id.username-{0}";
        public const string PROFILE_BY_SYSTEMNAME_KEY = "profile.systemname-{0}";
        public const string PROFILE_BY_USERNAME_KEY = "profile.username-{0}";
        public const string PROFILE_PATTERN_KEY = "profile.";

        public const string BRAND_ALL_ACTIVE_KEY = "brand.all.active";
        public const string BRAND_ALL_KEY = "brand.all";
        public const string BRAND_ACTIVE_BY_LETTER_KEY = "brand.active.letter-{0}";
        public const string BRAND_BY_ID_KEY = "brand.id-{0}";
        public const string BRAND_BY_CATEGORY_ID_KEY = "brandcategory.id-{0}";
        public const string BRAND_HAVING_MICROSITE = "brand.having-microsite";
        public const string BRAND_BY_URL_KEY = "brand.url-{0}";
        public const string BRAND_HAVING_ACTIVE_BRAND_CATEGORY_TREE_BY_ID_KEY = "brand.having-active-brand-category-by-id-{0}";
        public const string BRAND_MODEL_LETTER_KEY = "brand.model.letter-{0}";
        public const string BRAND_MODEL_URL_KEY = "brand.model.url-{0}";
        public const string BRAND_CATEGORY_BY_BRAND_ID_AND_PARENT_ID_KEY = "brandcategory.id-{0}.parent-id-{1}";
        public const string BRAND_CATEGORY_BY_ID_KEY = "brandcategory.id-{0}";
        public const string BRAND_CATEGORY_BY_URL_KEY = "brandcategory.url-{0}";
        public const string BRAND_CATEGORY_BY_PARENT_ID_KEY = "brandcategory.parent-id-{0}";        
        public const string BRAND_PATTERN_KEY = "brand.";
        public const string BRAND_CATEGORY_PATTERN_KEY = "brandcategory.";

        public const string LARGE_BANNER_ALL_ACTIVE_BY_DISPLAY_TYPE_KEY = "largebanner.all.active.display-type-{0}";
        public const string LARGE_BANNER_PATTERN_KEY = "largebanner.";

        public const string CATEGORY_BY_ID_KEY = "category.id-{0}";
        public const string CATEGORY_BY_URL_KEY = "category.url-{0}";
        public const string CATEGORY_BY_PARENT_ID_KEY = "category.parent-id-{0}";
        public const string CATEGORY_BY_PRODUCT_ID_KEY = "category.product-id-{0}";
        public const string CATEGORY_BY_LARGE_BANNER_ID_KEY = "category.large-banner-id-{0}";
        public const string CATEGORY_FOR_MENU = "category.menu";
        public const string CATEGOY_BY_TOP_SECOND_THIRD_URL_KEY = "category.top-{0}.second-{1}.third-{2}";
        public const string CATEGORY_TEMPLATE_BY_ID_KEY = "categorytemplate.id-{0}";
        public const string CATEGORY_TEMPLATE_ALL_KEY = "categorytemplate.all";
        public const string CATEGORY_FILTER_BY_CATEGORY_AND_BRAND_KEY = "categoryfilter.category-{0}.brand-{1}";
        public const string CATEGORY_FILTER_BY_ID_KEY = "categoryfilter.id-{0}";        
        public const string CATEGORY_TREE_LIST_WITH_NAME_BY_ID_KEY = "category.tree-list-with-name.id-{0}";
        public const string CATEGORY_GOOGLE_TAXONOMY_TREE_LIST_WITH_NAME_BY_GOOGLE_TAXONOMY_ID_KEY = "category.google-taxonomy-tree-list-with-name.google-taxonomy-id-{0}";
        public const string CATEGORY_PATTERN_KEY = "category.";
        public const string CATEGORY_FILTER_PATTERN_KEY = "categoryfilter.";

        public const string OFFER_RULE_BY_ID_URL_KEY = "offerrule.url-{0}";
        public const string OFFER_RULE_PATTERN_KEY = "offerrule.";
        public const string OFFER_RULE_ACTIVE_BY_TYPE_KEY = "offerrule.active.type-{0}";
        public const string OFFER_RULE_ALL_TYPES_KEY = "offerrule.all-types";
        public const string OFFER_RULE_ACTIVE_ORDER_BY_PRIORITY_KEY = "offerrule.active.order-by-priority-{0}";

        public const string ORDER_STATUS_CODE_KEY = "order.status-code-{0}";
        public const string ORDER_LINE_STATUS_CODE_KEY = "order.line-status-code-{0}";
        public const string ORDER_PATTERN_KEY = "order.";

        public const string LINE_ITEM_ALL_PICK_IN_PROGRESS = "lineitem.all-pick-in-progress";

        public const string PRODUCT_PRICE_BY_PRODUCT_PRICE_ID_KEY = "productprice.productprice-id-{0}";
        public const string PRODUCT_BY_URL_KEY = "product.url-{0}";
        public const string PRODUCT_BY_ID_KEY = "product.id-{0}";
        public const string PRODUCT_BY_PRODUCT_PRICE_ID_KEY = "product.product-price-id-{0}";
        public const string PRODUCT_OVERVIEW_BY_BRAND_KEY = "product.overview.brand-id-{0}";        
        public const string PRODUCT_OVERVIEW_BY_URL_KEY = "product.overview.url-{0}";
        public const string PRODUCT_BY_GROUP_ID = "product.group-id-{0}";
        public const string PRODUCT_PATTERN_KEY = "product.";
        public const string PRODUCT_PRICE_PATTERN_KEY = "productprice.";

        public const string COLOUR_BY_COLOUR_ID_KEY = "colour.colour-id-{0}";
        public const string COLOUR_PATTERN_KEY = "colour.";

        public const string SEARCH_PRODUCT_BY_FILTER_AND_PAGE_SIZE_AND_PAGE_INDEX_AND_QUERY = "searchproduct.filter-{0}.pagesize-{1}.pageindex-{2}.query-{3}";
        public const string SEARCH_TERM_BY_QUERY_KEY = "searchterm.query-{0}";
        public const string SEARCH_SUGGESTED_KEYWORD_BY_QUERY_KEY = "searchsuggestedkeyword.query-{0}";
        public const string SEARCH_TERM_PATTERN_KEY = "searchterm.";
        public const string SEARCH_PRODUCT_PATTERN_KEY = "searchproduct.";
        public const string SEARCH_SUGGESTED_KEYWORD_PATTERN_KEY = "searchsuggestedkeyword.";

        public const string SETTING_BY_TYPE_AND_STORE_ID = "setting.type-{0}.store-id-{1}";
        public const string SETTING_PATTERN_KEY = "setting.";
        public const string SETTING_ALL_KEY = "setting.all";

        public const string SHIPPING_OPTION_BY_ID_KEY = "shippingoption.id-{0}";
        public const string SHIPPING_OPTION_TRACKED_DELIVERY_ACTIVE = "shippingoption.tracked-delivery.active";
        public const string SHIPPING_OPTION_BY_COUNTRY_ID_AND_ENABLED = "shippingoption.country-id-{0}.enabled-{1}";

        public const string COUNTRY_ALL_KEY = "country.all";
        public const string COUNTRY_ALL_ACTIVE_KEY = "country.all.active";
        public const string COUNTRY_BY_COUNTRY_CODE_KEY = "country.country-code-{0}";
        public const string COUNTRY_BY_COUNTRY_ID_KEY = "country.country-id-{0}";
        public const string COUNTRY_PATTERN_KEY = "country.";

        public const string DELIVERY_ALL_KEY = "delivery.all";

        public const string USSTATE_BY_CODE_KEY = "usstate.code-{0}";
        public const string USSTATE_BY_ID_KEY = "usstate.id-{0}";
        public const string USSTATE_ALL_KEY = "usstate.all";
        public const string USSTATE_PATTERN_KEY = "usstate.";

        public const string SHIPPING_OPTION_PATTERN_KEY = "shippingoption.";
        public const string DELIVERY_PATTERN_KEY = "delivery.";
        
        public const string BLOG_MONTHS_MODEL_KEY = "blog.months";
        public const string BLOG_TAGS_MODEL_KEY = "blog.tags";
        public const string BLOG_PATTERN_KEY = "blog.";

        public const string CUSTOM_DICTIONARY_PATTERN_KEY = "customdictionary.";

        public const string RESTRICTED_GROUP_ACTIVE_BY_PRODUCT_ID_KEY = "restrictedgroup.active.product.id-{0}";
        public const string RESTRICTED_GROUP_PATTERN_KEY = "restrictedgroup.";

        public const string CHILD_ACTION_CACHE_ID_KEY = "child_action_cache.id-{0}";
        public const string CHILD_ACTION_CACHE_PATTERN_KEY = "child_action_cache.";

        public const string URL_PATH_KEY = "url.path-{0}.https-{1}";
        public const string URL_PATTERN_KEY = "url.";

        public const string WIDGET_PATTERN_KEY = "widget.";

        public const string BRANCH_ALL_KEY = "branch.all";
        public const string BRANCH_VECTOR_ID_BY_ID_KEY = "branch-vector-id.branch-id-{0}";
        public const string BRANCH_PATTERN_KEY = "branch.";
    }
}
