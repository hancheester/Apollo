using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Offer.OfferProcessor;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Apollo.Core.Services.Offer
{
    public class OfferService : BaseRepository, IOfferService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<OfferRule> _offerRuleRepository;
        private readonly IRepository<OfferAction> _offerActionRepository;
        private readonly IRepository<OfferActionAttribute> _offerActionAttributeRepository;
        private readonly IRepository<OfferCondition> _offerConditionRepository;
        private readonly IRepository<OfferOperator> _offerOperatorRepository;
        private readonly IRepository<OfferRelatedItem> _offerRelatedItemRepository;
        private readonly IRepository<OfferAttributeWithOperator> _offerAttributeWithOperatorRepository;
        private readonly IRepository<OfferAttribute> _offerAttributeRepository;
        private readonly IRepository<OfferType> _offerTypeRepository;
        private readonly ICartOfferProcessor _cartOfferProcessor;
        private readonly ICatalogOfferProcessor _catalogOfferProcessor;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public OfferService(IDbContext dbContext,
                            IRepository<Category> categoryRepository,
                            IRepository<OfferRule> offerRuleRepository,
                            IRepository<OfferAction> offerActionRepository,
                            IRepository<OfferActionAttribute> offerActionAttributeRepository,
                            IRepository<OfferCondition> offerConditionRepository,
                            IRepository<OfferOperator> offerOperatorRepository,
                            IRepository<OfferRelatedItem> offerRelatedItemRepository,
                            IRepository<OfferAttributeWithOperator> offerAttributeWithOperatorRepository,
                            IRepository<OfferAttribute> offerAttributeRepository,
                            IRepository<OfferType> offerTypeRepository,
                            IRepository<Product> productRepository,
                            ICartOfferProcessor cartOfferProcessor,
                            ICatalogOfferProcessor catalogOfferProcessor,
                            IGenericAttributeService genericAttributeService,
                            ILogBuilder logBuilder,
                            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
            _offerRuleRepository = offerRuleRepository;
            _offerActionRepository = offerActionRepository;
            _offerActionAttributeRepository = offerActionAttributeRepository;
            _offerConditionRepository = offerConditionRepository;
            _offerOperatorRepository = offerOperatorRepository;
            _offerRelatedItemRepository = offerRelatedItemRepository;
            _offerAttributeWithOperatorRepository = offerAttributeWithOperatorRepository;
            _offerAttributeRepository = offerAttributeRepository;
            _offerTypeRepository = offerTypeRepository;
            _productRepository = productRepository;
            _cartOfferProcessor = cartOfferProcessor;
            _catalogOfferProcessor = catalogOfferProcessor;
            _genericAttributeService = genericAttributeService;
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        #region Create

        public int InsertOfferRelatedItem(OfferRelatedItem item)
        {
            int id = _offerRelatedItemRepository.Create(item);
            return id;
        }

        public int InsertOfferCondition(OfferCondition condition)
        {
            return _offerConditionRepository.Create(condition);
        }
        
        #endregion

        #region Command
        
        public void RemoveCartOfferByPromoCode(int profileId, string promoCode)
        {
            _cartOfferProcessor.RemoveCartOfferByPromoCode(profileId, promoCode);
        }

        public CartOffer ProcessCartOfferByProfileId(int profileId, string shippingCountryCode, int testOfferRuleId = 0)
        {
            var gas = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");
            
            //get promocode
            var gaDiscountCouponCode = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.DiscountCouponCode);
            var discountCouponCode = string.Empty;
            if (gaDiscountCouponCode != null) discountCouponCode = gaDiscountCouponCode.Value;

            var cartOffer = _cartOfferProcessor.ProcessCart(profileId, shippingCountryCode, discountCouponCode, testOfferRuleId);
            return cartOffer;
        }
        
        public Product ProcessCatalog(Product product, int testOfferRuleId = 0)
        {
            product = _catalogOfferProcessor.ProcessCatalog(product, testOfferRuleId);
            return product;
        }
        
        public int ProcessOfferInsertion(OfferRule rule)
        {
            // Create offer rule
            int offerRuleId = _offerRuleRepository.Create(rule);

            // Create conditions
            InsertConditionWithOfferRuleId(rule.Condition, offerRuleId);

            // Create action
            rule.Action.OfferRuleId = offerRuleId;
            int offerActionId = _offerActionRepository.Create(rule.Action);

            // Create conditions from action (if available)
            if (rule.Action.Condition != null)
                InsertConditionWithOfferActionId(rule.Action.Condition, offerActionId);

            return offerRuleId;
        }

        public bool ProcessOfferUpdation(OfferRule rule)
        {
            // Update offer rule
            _offerRuleRepository.Update(rule);

            // Remove conditions
            var offerConditions = _offerConditionRepository.Table.Where(x => x.OfferRuleId == rule.Id).ToList();
            if (offerConditions != null)
            {
                foreach (var offerCondition in offerConditions)
                {
                    _offerConditionRepository.Delete(offerCondition);
                }
            }

            // Create conditions
            InsertConditionWithOfferRuleId(rule.Condition, rule.Id);

            // Update action
            _offerActionRepository.Update(rule.Action);

            // Remove action conditions
            var actionConditions = _offerConditionRepository.Table.Where(x => x.OfferActionId == rule.Action.Id).ToList();
            if (actionConditions != null)
            {
                foreach (var actionCondition in actionConditions)
                {
                    _offerConditionRepository.Delete(actionCondition);
                }
            }

            // Update action conditions (if available)
            if (rule.Action.Condition != null)
                InsertConditionWithOfferActionId(rule.Action.Condition, rule.Action.Id);

            // Clear cache
            _cacheManager.RemoveByPattern(CacheKey.OFFER_RULE_PATTERN_KEY);

            return false;
        }

        public void RemoveOfferedItemsFromBaskets(int offerRuleId = 0)
        {
            if (offerRuleId == 0)
                _dbContext.ExecuteSqlCommand("DELETE FROM CartItem WHERE OfferRuleId != 0;");
            else
            {
                var pOfferRuleId = GetParameter("OfferRuleId", offerRuleId);
                _dbContext.ExecuteSqlCommand("DELETE FROM CartItem WHERE OfferRuleId = @pOfferRuleId;", pOfferRuleId);
            }
        }

        public IList<OfferRuleOverviewModel> FindRelatedOffers(Product product)
        {
            return _cartOfferProcessor.FindRelatedOffers(product);
        }

        #endregion

        #region Return
        
        public OfferAttribute GetOfferAttribute(int offerAttributeId)
        {
            return _offerAttributeRepository.Return(offerAttributeId);
        }

        public OfferOperator GetOfferOperator(int offerOperatorId)
        {
            return _offerOperatorRepository.Return(offerOperatorId);
        }

        public IList<OfferActionAttribute> GetOfferActionAttributes(bool? isCatalog, bool? isCart)
        {
            var query = _offerActionAttributeRepository.Table;

            if (isCatalog.HasValue)
                query = query.Where(x => x.IsCatalog == isCatalog);

            if (isCart.HasValue)
                query = query.Where(x => x.IsCart == isCart);

            var items = query.ToList();

            return items;
        }

        public IList<OfferType> GetOfferTypes()
        {
            return _cacheManager.Get(CacheKey.OFFER_RULE_ALL_TYPES_KEY, delegate ()
            {
                return _offerTypeRepository.Table.ToList();
            });            
        }

        public IList<OfferTypeOverviewModel> GetActiveOfferTypes()
        {
            var items = _offerRuleRepository.TableNoTracking
                .Where(x => x.IsActive == true)
                .Where(x => x.StartDate == null || x.StartDate <= DateTime.Now)
                .Where(x => x.EndDate == null || x.EndDate >= DateTime.Now)
                .OrderBy(x => x.Priority)
                .Select(x => new { x.Id, x.OfferTypeId, x.ProceedForNext, x.ShowInOfferPage, x.Alias, x.UrlRewrite, x.IsCart })
                .ToList();

            // Get all offers before "Stop futher rules processings" flag is found.
            // Get all offers that should be in special offers page.

            if (items.Count == 0) return new List<OfferTypeOverviewModel>();

            var filterOfferRuleIds = new List<int>();
            foreach (var item in items)
            {
                if (item.ShowInOfferPage)
                    filterOfferRuleIds.Add(item.Id);

                if (item.ProceedForNext == false)
                    break;
            }

            items = items.Where(x => filterOfferRuleIds.Contains(x.Id)).ToList();

            if (items.Count == 0) return new List<OfferTypeOverviewModel>();

            var foundTypes = items.Where(x => x.OfferTypeId > 0).Select(x => x.OfferTypeId).Distinct();

            if (foundTypes.Count() == 0) return new List<OfferTypeOverviewModel>();

            var types = GetOfferTypes().Where(x => foundTypes.Contains(x.Id)).Select(x => new OfferTypeOverviewModel { Id = x.Id, Name = x.Type }).ToList();

            if (types.Count == 0) return new List<OfferTypeOverviewModel>();

            foreach (var type in types)
            {
                type.OfferRules = items.Where(x => x.OfferTypeId == type.Id)
                    .Select(x => new OfferRuleOverviewModel
                    {
                        Name = x.Alias,
                        UrlKey = x.UrlRewrite,
                        OfferRuleType = x.IsCart ? OfferRuleType.Cart : OfferRuleType.Catalog
                    })
                    .ToList();
            }

            return types;
        }

        public OfferRule GetOfferRuleOnlyById(int id)
        {
            return _offerRuleRepository.Return(id);
        }

        public OfferRule GetOfferRuleById(int id)
        {
            OfferRule rule = _offerRuleRepository.Return(id);
            rule = BuildOfferRule(rule);
            
            return rule;
        }

        public OfferRule GetOfferRuleByUrlKey(string urlKey)
        {
            OfferRule rule = _offerRuleRepository.TableNoTracking.Where(o => o.UrlRewrite == urlKey).FirstOrDefault();

            if (rule != null)
                rule = BuildOfferRule(rule);

            return rule;
        }

        public OfferRule GetOfferRuleByPromocode(string promocode)
        {
            OfferRule rule = _offerRuleRepository.TableNoTracking.Where(o => o.PromoCode == promocode).FirstOrDefault();
            if (rule != null)
                rule = BuildOfferRule(rule);

            return rule;
        }
        
        public IList<OfferCondition> GetOfferConditionListByOfferActionId(int offerActionId)
        {
            List<OfferCondition> list = _offerConditionRepository.Table.Where(c => c.OfferActionId == offerActionId).ToList();
            return list;
        }

        public IList<OfferCondition> GetOfferConditionListByOfferRuleId(int offerRuleId)
        {
            var conditions = _offerConditionRepository.Table.Where(c => c.OfferRuleId == offerRuleId).ToList();

            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    if (condition.OfferAttributeId.HasValue && condition.OfferAttributeId.Value > 0)                    
                        condition.OfferAttribute = _offerAttributeRepository.Return(condition.OfferAttributeId.Value);

                    if (condition.OfferOperatorId.HasValue && condition.OfferOperatorId.Value > 0)
                        condition.OfferOperator = _offerOperatorRepository.Return(condition.OfferOperatorId.Value);
                }
            }

            return conditions;
        }

        public IList<OfferOperator> GetOfferOperatorsByAttribute(int offerAttributeId)
        {
            var list = _offerOperatorRepository.Table
                .Join(_offerAttributeWithOperatorRepository.Table, oo => oo.Id, oawo => oawo.OfferOperatorId, (oo, oawa) => new { oo, oawa })
                .Where(oo_oawa => oo_oawa.oawa.OfferAttributeId == offerAttributeId)
                .Select(oo_oawa => oo_oawa.oo)
                .ToList();

            return list;
        }

        public IList<OfferAttribute> GetOfferAttributeByType(bool isCatalog = true, bool isCart = true)
        {
            var list = _offerAttributeRepository.Table.Where(a => a.IsCatalog == isCatalog && a.IsCart == isCart).ToList();
            return list;
        }

        public PagedList<OfferRule> GetOfferRuleLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> offerRuleIds = null,
            string name = null,
            string promocode = null,
            string fromDate = null,
            string toDate = null,
            bool? isActive = null,
            bool? isCart = null,
            bool? showInOfferPage = null, 
            int? offerTypeId = null,
            OfferRuleSortingType orderBy = OfferRuleSortingType.IdAsc)
        {
            var query = _offerRuleRepository.Table;

            if (offerRuleIds != null && offerRuleIds.Count > 0)
                query = query.Where(x => offerRuleIds.Contains(x.Id));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            if (!string.IsNullOrEmpty(promocode))
                query = query.Where(x => x.PromoCode.Contains(promocode));

            DateTime startDate;
            if (!string.IsNullOrEmpty(fromDate)
                && DateTime.TryParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                query = query.Where(x => startDate <= x.StartDate);
            }

            DateTime endDate;
            if (!string.IsNullOrEmpty(toDate)
                && DateTime.TryParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                query = query.Where(x => endDate >= x.EndDate);
            }
            
            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            if (isCart.HasValue)
                query = query.Where(x => x.IsCart == isCart.Value);

            if (showInOfferPage.HasValue)            
                query = query.Where(o => o.ShowInOfferPage == showInOfferPage.Value);

            if (offerTypeId.HasValue)
                query = query.Where(o => o.OfferTypeId == offerTypeId.Value);

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case OfferRuleSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case OfferRuleSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case OfferRuleSortingType.NameAsc:
                    query = query.OrderBy(x => x.Name);
                    break;
                case OfferRuleSortingType.NameDesc:
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case OfferRuleSortingType.PriorityAsc:
                    query = query.OrderBy(x => x.Priority);
                    break;
                case OfferRuleSortingType.PriorityDesc:
                    query = query.OrderByDescending(x => x.Priority);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<OfferRule>(list, pageIndex, pageSize, totalRecords);
        }
        
        public IList<OfferRelatedItem> GetOfferRelatedItemByOfferRuleId(int offerRuleId)
        {
            var list = _offerRelatedItemRepository.Table.Where(o => o.Id == offerRuleId).ToList();
            return list;
        }

        #endregion

        #region Delete

        public void DeleteOfferConditionByOfferConditionId(int offerConditionId)
        {
            _offerConditionRepository.Delete(offerConditionId);
        }

        public void DeleteOfferRelatedItem(int offerRelatedItemId)
        {
            _offerRelatedItemRepository.Delete(offerRelatedItemId);
        }

        #endregion

        #region Update

        public void UpdateOfferRelatedItem(OfferRelatedItem relatedItem)
        {
            _offerRelatedItemRepository.Update(relatedItem);
        }

        #endregion

        #region Private methods

        private OfferRule BuildOfferRule(OfferRule rule)
        {
            var conditions = (List<OfferCondition>)GetOfferConditionListByOfferRuleId(rule.Id);

            if (conditions != null && conditions.Count > 0)
            {
                OfferCondition parent = conditions.Find(delegate(OfferCondition item) { return item.ParentId == 0; });

                if (parent != null)
                {
                    rule.Condition = parent;
                    rule.Condition.ChildOfferConditions = BuildTreeOfferConditionInfoByParent(parent,
                                                                                              conditions,
                                                                                              parent.Id);
                }
            }

            rule.Action = GetOfferActionByOfferRuleId(rule.Id);
            
            rule.RelatedItems = _offerRelatedItemRepository.Table.Where(o => o.OfferRuleId == rule.Id).ToList();
            for (int i = 0; i < rule.RelatedItems.Count; i++)
            {
                var productId = rule.RelatedItems[i].ProductId;
                var product = _productRepository.Table.Where(x => x.Id == productId)
                    .Select(x => new { x.Name, x.OptionType })
                    .FirstOrDefault();
                
                if (product != null)
                {
                    rule.RelatedItems[i].ProductName = product.Name;
                }
            }

            return rule;
        }

        private OfferAction GetOfferActionByOfferRuleId(int offerRuleId)
        {
            OfferAction action = _offerActionRepository.Table.Where(a => a.OfferRuleId == offerRuleId).FirstOrDefault();

            if (action != null)
            {
                List<OfferCondition> conditions = (List<OfferCondition>)GetOfferConditionListByOfferActionId(action.Id);

                if (conditions != null && conditions.Count > 0)
                {
                    OfferCondition parent = conditions.Find(delegate (OfferCondition item) { return item.ParentId == 0; });

                    if (parent != null)
                    {
                        action.Condition = parent;
                        action.Condition.ChildOfferConditions = BuildTreeOfferConditionInfoByParent(parent,
                                                                                                  conditions,
                                                                                                  parent.Id);
                    }
                }

                if (action.OptionOperator != null)
                    action.OptionOperator = _offerOperatorRepository.Return(action.OptionOperatorId);
            }

            return action;
        }

        private void InsertConditionWithOfferRuleId(OfferCondition condition, int offerRuleId, int parentId = 0)
        {
            condition.OfferRuleId = offerRuleId;
            condition.ParentId = parentId;

            _offerConditionRepository.Create(condition);

            if (condition.ChildOfferConditions != null && condition.ChildOfferConditions.Count > 0)
            {
                foreach (var cond in condition.ChildOfferConditions)
                {
                    InsertConditionWithOfferRuleId(cond, offerRuleId, condition.Id);
                }
            }
        }

        private void InsertConditionWithOfferActionId(OfferCondition condition, int offerActionId, int parentId = 0)
        {
            condition.OfferActionId = offerActionId;
            condition.ParentId = parentId;
            _offerConditionRepository.Create(condition);

            if (condition.ChildOfferConditions != null && condition.ChildOfferConditions.Count > 0)
            {
                foreach (var cond in condition.ChildOfferConditions)
                {
                    InsertConditionWithOfferActionId(cond, offerActionId, condition.Id);
                }
            }
        }

        private List<OfferCondition> BuildTreeOfferConditionInfoByParent(OfferCondition target,
                                                                         List<OfferCondition> conditions,
                                                                         int parentId)
        {
            List<OfferCondition> list = conditions.FindAll(delegate(OfferCondition cond) { return cond.ParentId == parentId; });

            if (list != null && list.Count > 0)
                for (int i = 0; i < list.Count; i++)
                    list[i].ChildOfferConditions = BuildTreeOfferConditionInfoByParent(list[i], conditions, list[i].Id);

            return list;
        }

        #endregion
    }
}
