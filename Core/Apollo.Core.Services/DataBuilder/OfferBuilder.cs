using Apollo.Core.Caching;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.DataBuilder
{
    public class OfferBuilder : IOfferBuilder
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<OfferRule> _offerRuleRepository;
        private readonly IRepository<OfferAction> _offerActionRepository;
        private readonly IRepository<OfferCondition> _offerConditionRepository;
        private readonly IRepository<OfferOperator> _offerOperatorRepository;
        private readonly IRepository<OfferAttribute> _offerAttributeRepository;

        public OfferBuilder(
            ICacheManager cacheManager,
            IRepository<OfferRule> offerRuleRepository,
            IRepository<OfferAction> offerActionRepository,
            IRepository<OfferCondition> offerConditionRepository,
            IRepository<OfferOperator> offerOperatorRepository,
            IRepository<OfferAttribute> offerAttributeRepository)
        {
            _cacheManager = cacheManager;
            _offerRuleRepository = offerRuleRepository;
            _offerActionRepository = offerActionRepository;
            _offerConditionRepository = offerConditionRepository;
            _offerOperatorRepository = offerOperatorRepository;
            _offerAttributeRepository = offerAttributeRepository;
        }

        public IList<OfferRule> GetActiveOfferRulesByType(OfferRuleType type)
        {
            string key = string.Format(CacheKey.OFFER_RULE_ACTIVE_BY_TYPE_KEY, (int)type);
            var offers = _cacheManager.GetWithExpiry(key, delegate ()
            {
                var query = _offerRuleRepository.TableNoTracking
                .Where(x => x.IsActive == true);

                switch (type)
                {
                    case OfferRuleType.Catalog:
                        query = query.Where(x => x.IsCart == false);
                        break;
                    case OfferRuleType.Cart:
                        query = query.Where(x => x.IsCart == true);
                        break;
                    default:
                        break;
                }

                var rules = query.ToList();

                if (rules != null)
                {
                    // Order by priority
                    rules = rules.OrderBy(x => x.Priority).ToList();
                    for (int i = 0; i < rules.Count; i++)
                    {
                        rules[i] = BuildOfferRule(rules[i]);
                    }
                }

                return rules;
            },
            expiredEndOfDay: true);

            return offers;
        }

        public IList<OfferRule> CloneActiveOfferRulesByType(OfferRuleType type)
        {
            var rules = GetActiveOfferRulesByType(type);
            return rules.CloneJson();
        }

        public OfferRule PrepareTestOfferRule(int offerRuleId)
        {
            var rule = _offerRuleRepository.TableNoTracking.Where(x => x.Id == offerRuleId).FirstOrDefault();
            if (rule == null) return null;

            rule = BuildOfferRule(rule);
            // As this is a test offer, we have to make sure it is active and has no expiry date.
            rule.IsActive = true;
            rule.StartDate = null;
            rule.EndDate = null;

            return rule;
        }

        private OfferRule BuildOfferRule(OfferRule rule)
        {
            var conditions = _offerConditionRepository.TableNoTracking.Where(c => c.OfferRuleId == rule.Id).ToList();

            if (conditions != null && conditions.Count > 0)
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    conditions[i] = DetermineOfferConditionType(conditions[i]);
                }

                var parent = conditions.Find(delegate (OfferCondition item) { return item.ParentId == 0; });

                if (parent != null)
                {
                    rule.Condition = parent;
                    rule.Condition.ChildOfferConditions = BuildTreeOfferConditionInfoByParent(parent,
                                                                                              conditions,
                                                                                              parent.Id);
                }
            }

            rule.Action = GetOfferActionByOfferRuleId(rule.Id);

            return rule;
        }

        private OfferAction GetOfferActionByOfferRuleId(int offerRuleId)
        {
            var action = _offerActionRepository.TableNoTracking.Where(a => a.OfferRuleId == offerRuleId).FirstOrDefault();

            if (action != null)
            {
                var conditions = _offerConditionRepository.TableNoTracking.Where(c => c.OfferActionId == action.Id).ToList(); ;

                if (conditions != null && conditions.Count > 0)
                {
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        conditions[i] = DetermineOfferConditionType(conditions[i]);
                    }

                    var parent = conditions.Find(delegate (OfferCondition item) { return item.ParentId == 0; });

                    if (parent != null)
                    {
                        action.Condition = parent;
                        action.Condition.ChildOfferConditions = BuildTreeOfferConditionInfoByParent(parent, conditions, parent.Id);
                    }
                }

                action.OptionOperator = _offerOperatorRepository.Return(action.OptionOperatorId);
            }

            return action;
        }

        private List<OfferCondition> BuildTreeOfferConditionInfoByParent(OfferCondition target,
                                                                         List<OfferCondition> conditions,
                                                                         int parentId)
        {
            var list = conditions.FindAll(delegate (OfferCondition cond) { return cond.ParentId == parentId; });

            if (list != null && list.Count > 0)
                for (int i = 0; i < list.Count; i++)
                    list[i].ChildOfferConditions = BuildTreeOfferConditionInfoByParent(list[i], conditions, list[i].Id);

            return list;
        }

        private OfferCondition DetermineOfferConditionType(OfferCondition condition)
        {
            #region Is Item Matched?
            if (condition.IsAny.HasValue == true &&
                condition.IsAll.HasValue == true &&
                condition.Matched.HasValue == true &&
                condition.OfferAttributeId.HasValue == false &&
                condition.OfferOperatorId.HasValue == false &&
                condition.Operand == null &&
                condition.IsTotalQty.HasValue == false &&
                condition.IsTotalAmount.HasValue == false &&
                condition.ItemFound.HasValue == true)
            {
                condition.Type = OfferConditionType.ItemMatched;

                return condition;
            }
            #endregion

            #region Is Subselection?
            if (condition.IsAny.HasValue == true &&
                condition.IsAll.HasValue == true &&
                condition.Matched.HasValue == true &&
                condition.OfferAttributeId.HasValue == true &&
                condition.OfferOperatorId.HasValue == true &&
                condition.Operand != null &&
                condition.IsTotalQty.HasValue == true &&
                condition.IsTotalAmount.HasValue == true &&
                condition.ItemFound.HasValue == false)
            {
                condition.Type = OfferConditionType.Subselection;

                condition.OfferAttribute = GetOfferAttribute(condition.OfferAttributeId.Value);
                condition.OfferOperator = GetOfferOperator(condition.OfferOperatorId.Value);

                return condition;
            }
            #endregion

            #region Is Attribute?
            if (condition.IsAll.HasValue == false &&
                condition.IsAny.HasValue == false &&
                condition.Matched.HasValue == false &&
                condition.OfferAttributeId.HasValue == true &&
                condition.OfferOperatorId.HasValue == true &&
                condition.Operand != null &&
                condition.IsTotalQty.HasValue == false &&
                condition.IsTotalAmount.HasValue == false &&
                condition.ItemFound.HasValue == false)
            {
                condition.Type = OfferConditionType.Attribute;

                condition.OfferAttribute = GetOfferAttribute(condition.OfferAttributeId.Value);
                condition.OfferOperator = GetOfferOperator(condition.OfferOperatorId.Value);

                return condition;
            }
            #endregion

            return condition;
        }

        private OfferAttribute GetOfferAttribute(int offerAttributeId)
        {
            return _offerAttributeRepository.Return(offerAttributeId);
        }

        private OfferOperator GetOfferOperator(int offerOperatorId)
        {
            return _offerOperatorRepository.Return(offerOperatorId);
        }        
    }
}
