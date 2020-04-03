using Apollo.Core.Caching;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public class AttributeUtility
    {
        private const char SPLITTER = ',';

        private const string IS = "is";
        private const string IS_NOT = "is not";
        private const string EQUALS_OR_GREATER_THAN = "equals or greater than";
        private const string EQUALS_OR_LESS_THAN = "equals or less than";
        private const string GREATER_THAN = "greater than";
        private const string LESS_THAN = "less than";
        private const string CONTAINS = "contains";
        private const string DOES_NOT_CONTAIN = "does not contain";
        private const string IS_ONE_OF = "is one of";
        private const string IS_NOT_ONE_OF = "is not one of";

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<BrandCategory> _brandCategoryRepository;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public AttributeUtility(
            IDbContext dbContext,
            ICacheManager cacheManager,
            IRepository<Product> productRepository,
            IRepository<Brand> brandRepository,
            IRepository<BrandCategory> brandCategoryRepository)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _brandCategoryRepository = brandCategoryRepository;
        }

        public string GetAttributeValue(int attributeId, IList<CartItem> items, bool offeredItemIncluded, bool useInitialPrice, bool putInitialPriceInBasket)
        {
            string value = string.Empty;

            switch ((OfferAttributeType)attributeId)
            {
                case OfferAttributeType.SUBTOTAL:
                    decimal subtotal = 0M;

                    var querySubtotal = items.Where(x => x.Product.OpenForOffer == true);

                    if (offeredItemIncluded == false)
                        querySubtotal = querySubtotal.Where(x => x.ProductPrice.OfferRuleId <= 0);

                    var list = querySubtotal.ToList();

                    // If offer has flagged UseInitialPrice and been allowed to update lines in basket
                    if (useInitialPrice && putInitialPriceInBasket)
                    {
                        foreach (var item in items)
                        {
                            var foundItem = list.Find(x => x.Id == item.Id);
                            if (foundItem != null) foundItem.CartItemMode = (int)CartItemMode.InitialPrice;
                        }
                    }

                    if (useInitialPrice)
                        subtotal = list.Select(x => x.ProductPrice.PriceInclTax * x.Quantity).DefaultIfEmpty(0).Sum();
                    else
                        subtotal = list.Select(x => x.ProductPrice.OfferPriceInclTax * x.Quantity).DefaultIfEmpty(0).Sum();

                    value = subtotal.ToString();
                    break;

                case OfferAttributeType.TOTAL_ITEMS_QTY:
                    int qty = 0;

                    var queryQty = items.Where(x => x.Product.OpenForOffer == true);

                    if (offeredItemIncluded == false)
                        queryQty.Where(x => x.ProductPrice.OfferRuleId <= 0);

                    qty = queryQty.Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

                    value = qty.ToString();
                    break;
            }

            return value;
        }

        public bool ValidateAttribute(string value, string offerOperator, string operand)
        {
            string[] list;
            string[] valueList;

            decimal decimalOperand1 = 0M;
            decimal decimalOperand2 = 0M;

            switch (offerOperator)
            {
                case IS:
                    valueList = value.Split(SPLITTER);

                    for (int i = 0; i < valueList.Length; i++)
                        if (valueList[i].ToLower().CompareTo(operand.ToLower()) == 0)
                            return true;

                    return false;

                case IS_NOT:
                    valueList = value.Split(SPLITTER);

                    for (int i = 0; i < valueList.Length; i++)
                        if (valueList[i].ToLower().CompareTo(operand.ToLower()) != 0)
                            return true;

                    return false;

                case EQUALS_OR_GREATER_THAN:
                    if (decimal.TryParse(value, out decimalOperand1) && decimal.TryParse(operand, out decimalOperand2))
                        return decimalOperand1 >= decimalOperand2;

                    return false;

                case EQUALS_OR_LESS_THAN:
                    if (decimal.TryParse(value, out decimalOperand1) && decimal.TryParse(operand, out decimalOperand2))
                        return decimalOperand1 <= decimalOperand2;

                    return false;

                case GREATER_THAN:
                    if (decimal.TryParse(value, out decimalOperand1) && decimal.TryParse(operand, out decimalOperand2))
                        return decimalOperand1 > decimalOperand2;

                    return false;

                case LESS_THAN:
                    if (decimal.TryParse(value, out decimalOperand1) && decimal.TryParse(operand, out decimalOperand2))
                        return decimalOperand1 < decimalOperand2;

                    return false;

                case CONTAINS:
                    list = operand.Split(SPLITTER);

                    for (int i = 0; i < list.Length; i++)
                        if (value.Contains(list[i].ToLower()))
                            return true;
                    return false;

                case DOES_NOT_CONTAIN:
                    list = operand.Split(SPLITTER);

                    for (int i = 0; i < list.Length; i++)
                        if (value.Contains(list[i].ToLower()))
                            return false;
                    return true;

                case IS_ONE_OF:
                    list = operand.Split(SPLITTER);
                    valueList = value.Split(SPLITTER);

                    for (int i = 0; i < list.Length; i++)
                        for (int j = 0; j < valueList.Length; j++)
                            if (list[i] == valueList[j])
                                return true;

                    return false;

                case IS_NOT_ONE_OF:
                    list = operand.Split(SPLITTER);
                    valueList = value.Split(SPLITTER);

                    for (int i = 0; i < list.Length; i++)
                        for (int j = 0; j < valueList.Length; j++)
                            if (list[i] == valueList[j])
                                return false;

                    return true;
            }

            return false;
        }

        public string GetAttributeValue(int attributeId, Product product)
        {
            if (product == null) return string.Empty;

            string value = string.Empty;

            switch ((OfferAttributeType)attributeId)
            {
                case OfferAttributeType.PRODUCT_ID:
                    value = product.Id.ToString();
                    break;

                case OfferAttributeType.NAME:
                    value = product.Name.ToLower();
                    break;

                case OfferAttributeType.BRAND_ID:
                    value = product.BrandId.ToString();
                    break;

                case OfferAttributeType.BRAND:
                    if (product.BrandId > 0)
                    {
                        if (product.Brand != null)
                            value = product.Brand.Name.ToLower();
                        else
                        {
                            var brand = _brandRepository.Return(product.BrandId);
                            if (brand != null)
                                value = brand.Name.ToLower();
                        }
                    }
                    break;

                case OfferAttributeType.BRAND_CATEGORY:
                    if (product.BrandCategoryId > 0)
                    {
                        var brandCat = _brandCategoryRepository.Return(product.BrandCategoryId);

                        if (brandCat != null)
                            value = brandCat.Name;
                    }
                    break;

                case OfferAttributeType.BRAND_CATEGORY_ID:
                    value = product.BrandCategoryId.ToString();
                    break;

                case OfferAttributeType.CATEGORY_ID:
                    value = string.Join(SPLITTER.ToString(), GetCategoryIdsByProductId(product.Id));
                    break;
            }

            return value;
        }

        public string GetAttributeValue(int attributeId, int productId)
        {
            string value = string.Empty;
            var product = _productRepository.Return(productId);

            return GetAttributeValue(attributeId, product);
        }

        public string[] GetCategoryIdsByProductId(int productId)
        {
            var result = Retry.Do(delegate ()
            {
                string key = string.Format(CacheKey.CATEGORY_BY_PRODUCT_ID_KEY, productId);

                var categories = _cacheManager.Get(key, () => {
                    var pProductId = GetParameterWithValue("ProductId", productId, DbType.Int32);
                    var ids = _dbContext.SqlQuery<int>("EXEC CategoryId_GetByProductId @ProductId", pProductId);

                    if (ids == null) return new string[0];
                    return ids.Select(x => x.ToString()).ToArray();
                });

                return categories;

            }, TimeSpan.FromSeconds(3));

            return result;
        }

        public SqlParameter GetParameterWithValue(string name, object value, DbType type)
        {
            var param = new SqlParameter(name, DBNull.Value)
            {
                Value = value,
                DbType = type
            };

            return param;
        }
    }
}
