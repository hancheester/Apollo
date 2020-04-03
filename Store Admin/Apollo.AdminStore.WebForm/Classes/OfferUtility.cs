using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class OfferUtility
    {
        const string PIPE = "|";
        const string UL = "ul";
        const string LI = "li";
        const string ID = "id";        
        const string TRUE = "1";
        const string FALSE = "0";
        const string SELECT = "select";
        const string OPTION = "option";
        const string VALUE = "value";
        const string CART = "cart";
        const string CATALOG = "catalog";
        const string ACTION = "action";
        const string OFFER_CONDITION = "OfferCondition";
        const string OFFER_ACTION_CONDITION = "OfferActionCondition";
        const string EXISTING_OFFER_RULE_CONDITIONS = "ExistingOfferRuleConditions";
        const string EXISTING_OFFER_ACTION_CONDITIONS = "ExistingOfferActionConditions";        
        const string MONITOR_VALUE_FORMAT = "monitor_value('{0}', '{1}', this.value)";
        const string UPDATE_ALL_ANY = "update_cond1";
        const string UPDATE_ALL_ANY_FORMAT = "update_cond1('{0}', this.value, '{1}');";
        const string UPDATE_MATCHED = "update_cond2";
        const string UPDATE_MATCHED_FORMAT = "update_cond2('{0}', this.value, '{1}');";
        const string UPDATE_ITEM_FOUND = "update_cond3";
        const string UPDATE_ITEM_FOUND_FORMAT = "update_cond3('{0}', this.value, '{1}');";
        const string UPDATE_QTY_AMOUNT = "update_cond4";
        const string UPDATE_QTY_AMOUNT_FORMAT = "update_cond4('{0}', this.value, '{1}');";
        const string REMOVE_CONDITION_FORMAT = "javascript:remove_condition('{0}', '{1}');";
        const string UPDATE_OPERAND = "update_operand";
        const string UPDATE_OPERAND_FORMAT = "javascript:update_operand('{0}', '{1}', '{2}', '{3}');";
        const string UPDATE_OPERATOR = "update_operator";
        const string UPDATE_OPERATOR_FORMAT = "update_operator('{0}', this.value, '{1}');";
        const string TOGGLE_SELECTION_FORMAT = "toggle_selection('{0}');";
        const string LOAD_CONDITION = "load_condition";
        const string LOAD_CONDITION_FORMAT = "load_condition('{0}', this.value, '{1}');";
        const string DELETE_CONDITION = "delete_condition";

        const string OFFER_DEFAULT = "default";
        const int OFFER_CONDITIONS_COMBINATION = -1;
        const int OFFER_PRODUCT_ATTR_COMBINATION = -2;  // For cart only
        const int OFFER_PRODUCTS_SUBSELECTION = -3;     // For cart only
        
        public OfferConditionCollection OfferRuleConditions
        {
            get
            {
                if (HttpContext.Current.Session[EXISTING_OFFER_RULE_CONDITIONS] == null)
                    HttpContext.Current.Session[EXISTING_OFFER_RULE_CONDITIONS] = new OfferConditionCollection();

                return (OfferConditionCollection)HttpContext.Current.Session[EXISTING_OFFER_RULE_CONDITIONS];
            }
            set { HttpContext.Current.Session[EXISTING_OFFER_RULE_CONDITIONS] = value; }
        }
        public OfferConditionCollection OfferActionConditions
        {
            get
            {
                if (HttpContext.Current.Session[EXISTING_OFFER_ACTION_CONDITIONS] == null)
                    HttpContext.Current.Session[EXISTING_OFFER_ACTION_CONDITIONS] = new OfferConditionCollection();

                return (OfferConditionCollection)HttpContext.Current.Session[EXISTING_OFFER_ACTION_CONDITIONS];
            }
            set { HttpContext.Current.Session[EXISTING_OFFER_ACTION_CONDITIONS] = value; }
        }

        private IOfferService _offerService;
        private ILogger _logger;

        public OfferUtility(IOfferService offerService, ILogBuilder logBuilder)
        {
            if (offerService == null) throw new ArgumentException("offerService");
            if (logBuilder == null) throw new ArgumentException("logBuilder");

            this._offerService = offerService;
            this._logger = logBuilder.CreateLogger(typeof(OfferUtility).FullName);
        }

        public void ProcessActionCondition(HttpRequest request, HttpResponse response, string treeId, string type)
        {
            var root = OfferActionConditions[treeId];
            OfferActionConditions[treeId] = ProcessOfferConditionHandler(request, response, root, type);
        }

        public void ProcessOfferRuleCondition(HttpRequest request, HttpResponse response, string treeId, string type)
        {
            var root = OfferRuleConditions[treeId];
            OfferRuleConditions[treeId] = ProcessOfferConditionHandler(request, response, root, type);
        }

        #region Rendering methods

        public void RenderCondition(OfferCondition condition, HtmlTextWriter writer, OfferRenderType offerType)
        {
            string type = string.Empty;
            bool isCart = false;

            switch (offerType)
            {
                case OfferRenderType.Cart:
                    type = CART;
                    isCart = true;
                    break;
                case OfferRenderType.Catalog:
                    type = CATALOG;
                    break;
                case OfferRenderType.Action:
                default:
                    type = ACTION;
                    break;
            }

            if (condition.IsAll.HasValue && condition.IsAny.HasValue && condition.Matched.HasValue)
            {
                if (IsProductAttrCombination(condition))
                {
                    RenderProdAttrCombinationCondition(elementId: condition.Id.ToString(),
                                                       type: type,
                                                       itemFound: condition.ItemFound.Value,
                                                       isAny: condition.IsAny.Value,
                                                       enclosed: isCart,
                                                       writer: writer);

                    isCart = false;
                }
                else if (IsProductSubselection(condition))
                {
                    RenderProdSubselectionCondition(elementId: condition.Id.ToString(),
                                                    type: type,
                                                    operatorId: condition.OfferOperator.Id,
                                                    operand: condition.Operand,
                                                    isQuantity: condition.IsTotalQty.Value,
                                                    isAmount: condition.IsTotalAmount.Value,
                                                    isAny: condition.IsAny.Value,
                                                    enclosed: isCart,
                                                    writer: writer);

                    isCart = false;
                }
                else
                {
                    RenderCombinationCondition(elementId: condition.Id.ToString(),
                                               type: type,
                                               isAny: condition.IsAny.Value,
                                               matched: condition.Matched.Value,
                                               enclosed: isCart,
                                               writer: writer);
                }

                writer.WriteBeginTag(UL);
                writer.Write(HtmlTextWriter.TagRightChar);
                if (condition.ChildOfferConditions != null)
                {
                    for (int i = 0; i < condition.ChildOfferConditions.Count; i++)
                        RenderCondition(condition.ChildOfferConditions[i], writer, offerType);
                }

                RenderNewSelection(condition.Id.ToString(), type, isCart, writer);
                writer.WriteEndTag(UL);

                writer.WriteEndTag(LI);
            }
            else
            {
                string title = GetTitleByAttributeId(condition.OfferAttributeId.Value);
                int operatorId = condition.OfferOperatorId.Value;
                var list = _offerService.GetOfferOperatorsByAttribute(condition.OfferAttributeId.Value);

                RenderOperation(title, condition.Id.ToString(), type, operatorId, condition.Operand, condition.OfferAttributeId.Value, writer);
            }
        }

        private void RenderProdSubselectionCondition(string elementId, string type, int operatorId, string operand, bool isQuantity, bool isAmount, bool isAny, bool enclosed, HtmlTextWriter writer)
        {
            var operandInputId = elementId + "|operand";
            var updateCallerId = elementId + "|update|" + type;

            writer.WriteBeginTag(LI);
            writer.WriteAttribute(ID, elementId);
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.Write("if");

            writer.Write(HtmlTextWriter.SpaceChar);

            #region Render select for total quantity/total amount
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_QTY_AMOUNT_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            if (isQuantity) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, TRUE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("total quantity");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            if (!isQuantity) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, FALSE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("total amount");
            writer.WriteEndTag(OPTION);

            writer.WriteEndTag(SELECT);
            #endregion

            writer.Write(HtmlTextWriter.SpaceChar);

            var attributeId = 0;
            if (isQuantity)
                attributeId = (int)OfferAttributeType.TOTAL_ITEMS_QTY;
            else if (isAmount)
                attributeId = (int)OfferAttributeType.SUBTOTAL;

            RenderOperatorByAttribute(elementId, type, operatorId, attributeId, writer);

            writer.Write(HtmlTextWriter.SpaceChar);

            writer.WriteBeginTag("input");
            writer.WriteAttribute(ID, operandInputId);
            writer.WriteAttribute("type", "text");
            writer.WriteAttribute(VALUE, operand);
            writer.WriteAttribute("onkeyup", string.Format(MONITOR_VALUE_FORMAT, updateCallerId, operand));
            writer.WriteEndTag("input");

            writer.Write(HtmlTextWriter.SpaceChar);

            writer.Write("for a subselection of items in cart matching");

            writer.Write(HtmlTextWriter.SpaceChar);

            #region Render select for ALL/ANY
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_ALL_ANY_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            if (!isAny) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, FALSE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("ALL");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            if (isAny) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, TRUE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("ANY");
            writer.WriteEndTag(OPTION);

            writer.WriteEndTag(SELECT);
            #endregion

            writer.Write(HtmlTextWriter.SpaceChar);

            writer.Write("of these conditions");

            writer.Write(HtmlTextWriter.SpaceChar);
            
            writer.WriteBeginTag("a");
            writer.WriteAttribute(ID, updateCallerId);
            writer.WriteAttribute("href", string.Format(UPDATE_OPERAND_FORMAT, elementId, operandInputId, type, updateCallerId));
            writer.WriteAttribute("style", "display: none;");
            writer.Write(HtmlTextWriter.TagRightChar);            
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-floppy-o save");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            writer.WriteEndTag("a");
            
            writer.WriteBeginTag("a");
            writer.WriteAttribute("href", string.Format(REMOVE_CONDITION_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);            
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-trash remove");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            writer.WriteEndTag("a");

            if (enclosed) writer.WriteEndTag(LI);
        }

        private void RenderProdAttrCombinationCondition(string elementId, string type, bool itemFound, bool isAny, bool enclosed, HtmlTextWriter writer)
        {
            writer.WriteBeginTag(LI);
            writer.WriteAttribute(ID, elementId);
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.Write("if an item is");
            writer.Write(HtmlTextWriter.SpaceChar);

            // Render select for FOUND/NOT FOUND
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_ITEM_FOUND_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            if (itemFound) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, TRUE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("FOUND");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            if (!itemFound) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, FALSE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("NOT FOUND");
            writer.WriteEndTag(OPTION);

            writer.WriteEndTag(SELECT);
            // End of Render select for FOUND/NOT FOUND

            writer.Write(HtmlTextWriter.SpaceChar);
            writer.Write("in the cart with");
            writer.Write(HtmlTextWriter.SpaceChar);

            // Render select for ALL/ANY
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_ALL_ANY_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            if (!isAny) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, FALSE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("ALL");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            if (isAny) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, TRUE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("ANY");
            writer.WriteEndTag(OPTION);

            writer.WriteEndTag(SELECT);
            // End of Render select for ALL/ANY

            writer.Write(HtmlTextWriter.SpaceChar);
            writer.Write("of these conditions true");
            writer.Write(HtmlTextWriter.SpaceChar);

            writer.WriteBeginTag("a");
            writer.WriteAttribute("href", string.Format(REMOVE_CONDITION_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);            
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-trash remove");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            writer.WriteEndTag("a");

            if (enclosed) writer.WriteEndTag(LI);
        }

        private void RenderCombinationCondition(string elementId, string type, bool isAny, bool matched, bool enclosed, HtmlTextWriter writer)
        {
            writer.WriteBeginTag(LI);            
            writer.WriteAttribute(ID, elementId);
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.Write("if");
            writer.Write(HtmlTextWriter.SpaceChar);

            // Render select for ALL/ANY
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_ALL_ANY_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            if (!isAny) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, FALSE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("ALL");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            if (isAny) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, TRUE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("ANY");
            writer.WriteEndTag(OPTION);

            writer.WriteEndTag(SELECT);
            // End of Render select for ALL/ANY

            writer.Write(HtmlTextWriter.SpaceChar);
            writer.Write("conditions are");
            writer.Write(HtmlTextWriter.SpaceChar);

            // Render select for TRUE/FALSE
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_MATCHED_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            if (matched) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, TRUE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("TRUE");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            if (!matched) writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, FALSE);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("FALSE");
            writer.WriteEndTag(OPTION);

            writer.WriteEndTag(SELECT);
            // End of Render select for TRUE/FALSE

            writer.Write(HtmlTextWriter.SpaceChar);

            writer.WriteBeginTag("a");
            writer.WriteAttribute("href", string.Format(REMOVE_CONDITION_FORMAT, elementId, type));            
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-trash remove");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            writer.WriteEndTag("a");

            if (enclosed) writer.WriteEndTag(LI);
        }

        private void RenderProductOptionGroup(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("optgroup");
            writer.WriteAttribute("label", "Product Attribute");
            writer.Write(HtmlTextWriter.TagRightChar);

            // Get catalog only attributes
            var attributes = _offerService.GetOfferAttributeByType(isCatalog: true, isCart: false);

            for (int i = 0; i < attributes.Count; i++)
            {
                writer.WriteBeginTag(OPTION);
                writer.WriteAttribute(VALUE, attributes[i].Id.ToString());
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(attributes[i].Value);
                writer.WriteEndTag(OPTION);
            }

            writer.WriteEndTag("optgroup");
        }

        private void RenderCartOptionGroup(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("optgroup");
            writer.WriteAttribute("label", "Cart Attribute");
            writer.Write(HtmlTextWriter.TagRightChar);

            // Get cart only attributes
            var attributes = _offerService.GetOfferAttributeByType(isCatalog: false, isCart: true);

            for (int i = 0; i < attributes.Count; i++)
            {
                writer.WriteBeginTag(OPTION);
                writer.WriteAttribute(VALUE, attributes[i].Id.ToString());
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(attributes[i].Value);
                writer.WriteEndTag(OPTION);
            }

            writer.WriteEndTag("optgroup");
        }

        private void RenderOperation(string title, string elementId, string type, int operatorId, string operand, IList<OfferOperator> attributeOperators, HtmlTextWriter writer)
        {
            var operandInputId = elementId + "|operand";
            var updateCallerId = elementId + "|update|" + type;

            writer.WriteBeginTag(LI);
            writer.WriteAttribute(ID, elementId);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write(title);

            RenderOperatorByAttribute(elementId, type, operatorId, attributeOperators, writer);

            writer.Write(HtmlTextWriter.SpaceChar);

            writer.WriteBeginTag("input");
            writer.WriteAttribute(ID, operandInputId);
            writer.WriteAttribute("type", "text");
            writer.WriteAttribute(VALUE, operand);
            writer.WriteAttribute("onkeyup", string.Format(MONITOR_VALUE_FORMAT, updateCallerId, operand));
            writer.WriteEndTag("input");

            writer.Write(HtmlTextWriter.SpaceChar);

            writer.WriteBeginTag("a");
            writer.WriteAttribute(ID, updateCallerId);
            writer.WriteAttribute("href", string.Format(UPDATE_OPERAND_FORMAT, elementId, operandInputId, type, updateCallerId));
            writer.WriteAttribute("style", "display: none;");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-floppy-o save");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            writer.WriteEndTag("a");

            //writer.WriteBeginTag("a");
            //writer.WriteAttribute(ID, elementId + PIPE + "display");            
            //writer.WriteAttribute("href", "javascript:display_operand('" + elementId + PIPE + "update','" + elementId + "', '" + elementId + PIPE + "operand" + "', '" + type + "');");            
            //writer.Write(HtmlTextWriter.TagRightChar);                        
            //writer.WriteBeginTag("i");            
            //writer.WriteAttribute("class", "fa fa-table table");
            //writer.Write(HtmlTextWriter.TagRightChar);
            //writer.WriteEndTag("i");
            //writer.WriteEndTag("a");

            //writer.WriteBeginTag("a");
            //writer.WriteAttribute(ID, elementId + PIPE + "update");
            //writer.WriteAttribute("href", "javascript:apply_operand('" + elementId + "', this.value, '" + type + "');");            
            //writer.Write(HtmlTextWriter.TagRightChar);                        
            //writer.WriteBeginTag("i");
            //writer.WriteAttribute("class", "fa fa-check-circle confirm");
            //writer.Write(HtmlTextWriter.TagRightChar);
            //writer.WriteEndTag("i");
            //writer.WriteEndTag("a");

            writer.WriteBeginTag("a");
            writer.WriteAttribute("href", string.Format(REMOVE_CONDITION_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-trash remove");
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            writer.WriteEndTag("a");

            writer.WriteEndTag(LI);
        }

        private void RenderOperation(string title, string elementId, string type, int operatorId, string operand, int attributeId, HtmlTextWriter writer)
        {
            var operators = _offerService.GetOfferOperatorsByAttribute(attributeId);

            if (operators == null)
                _logger.InsertLog(LogLevel.Error, string.Format("Operators could not be loaded by this attribute. Offer Attribute ID = {{{0}}}", attributeId));
            else
                RenderOperation(title, elementId, type, operatorId, operand, operators, writer);            
        }

        private void RenderOperatorByAttribute(string elementId, string type, int operatorId, IList<OfferOperator> attributeOperators, HtmlTextWriter writer)
        {
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onchange", string.Format(UPDATE_OPERATOR_FORMAT, elementId, type));
            writer.Write(HtmlTextWriter.TagRightChar);

            for (int i = 0; i < attributeOperators.Count; i++)
            {
                writer.WriteBeginTag(OPTION);
                writer.WriteAttribute(VALUE, attributeOperators[i].Id.ToString());

                if (operatorId == attributeOperators[i].Id)
                    writer.WriteAttribute("selected", "selected");

                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(attributeOperators[i].Operator);
                writer.WriteEndTag(OPTION);
            }

            writer.WriteEndTag(SELECT);
        }

        private void RenderOperatorByAttribute(string elementId, string type, int operatorId, int attributeId, HtmlTextWriter writer)
        {
            var operators = _offerService.GetOfferOperatorsByAttribute(attributeId);

            if (operators == null)
                _logger.InsertLog(LogLevel.Error, string.Format("Operators could not be loaded by this attribute. Offer Attribute ID = {{{0}}}", attributeId));
            else
                RenderOperatorByAttribute(elementId, type, operatorId, operators, writer);
        }

        // Render new dropdown next to circle-plus symbol
        private void RenderNewSelection(string elementId, string type, bool isCart, HtmlTextWriter writer)
        {
            var newId = elementId + "|new" + type;

            writer.WriteBeginTag(LI);
            writer.WriteAttribute(ID, newId);
            writer.Write(HtmlTextWriter.TagRightChar);
            
            writer.WriteBeginTag("i");
            writer.WriteAttribute("class", "fa fa-plus-circle more");
            writer.WriteAttribute("onclick", string.Format(TOGGLE_SELECTION_FORMAT, newId));
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteEndTag("i");
            
            writer.WriteBeginTag(SELECT);
            writer.WriteAttribute("onblur", string.Format(TOGGLE_SELECTION_FORMAT, newId));
            writer.WriteAttribute("onchange", string.Format(LOAD_CONDITION_FORMAT, newId, type));
            writer.WriteAttribute("style", "display: none;");
            writer.Write(HtmlTextWriter.TagRightChar);

            writer.WriteBeginTag(OPTION);
            writer.WriteAttribute("selected", "selected");
            writer.WriteAttribute(VALUE, OFFER_DEFAULT);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("Please choose a condition to add");
            writer.WriteEndTag(OPTION);

            writer.WriteBeginTag(OPTION);
            writer.WriteAttribute(VALUE, OFFER_CONDITIONS_COMBINATION.ToString());
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write("Conditions Combination");
            writer.WriteEndTag(OPTION);

            if (isCart)
            {
                writer.WriteBeginTag(OPTION);
                writer.WriteAttribute(VALUE, OFFER_PRODUCT_ATTR_COMBINATION.ToString());
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write("Product Attribute Combination");
                writer.WriteEndTag(OPTION);

                writer.WriteBeginTag(OPTION);
                writer.WriteAttribute(VALUE, OFFER_PRODUCTS_SUBSELECTION.ToString());
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write("Products Subselection");
                writer.WriteEndTag(OPTION);

                RenderCartOptionGroup(writer);
            }
            else            
                RenderProductOptionGroup(writer);

            writer.WriteEndTag(SELECT);
            writer.WriteEndTag(LI);
        }
        
        #endregion
        
        #region Private methods

        private OfferCondition ProcessOfferConditionHandler(HttpRequest request, HttpResponse response, OfferCondition root, string type)
        {
            string elementId = request.Form["elementId"];
            int nodeId = Convert.ToInt32(elementId.Split('|')[0]); //It could be "id|new.." or just "id".
            string data = request.Form["data"];
            string action = request.Form[ACTION];
            string responseData = "ok";

            // If elementId contains "|new..", it means we need to create a new condition which the parent 
            // is from id extracted from "id|new".
            // In other words, we will find the node by id. Once the node is found, we create a child node for it.
            var node = SearchCondition(root, nodeId);
            if (node == null)
            {
                responseData = "Condition could not be found. Please contact administrator.";
                response.Clear();
                response.Write(responseData);
                response.End();
                return root;
            }

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            
            switch (action)
            {
                case LOAD_CONDITION:                    
                    var isCart = type == CART;
                    // Maximum possible value is 235959999, Int32.MaxValue is 2147483647
                    var newId = Convert.ToInt32(DateTime.Now.ToString("HHmmssfff"));
                    
                    // Prepare child list of the found node
                    int index = 0;

                    if ((node.ChildOfferConditions == null) || (node.ChildOfferConditions.Count == 0))
                    {
                        node.ChildOfferConditions = new List<OfferCondition>();
                        node.ChildOfferConditions.Add(new OfferCondition { Id = newId });
                    }
                    else
                    {
                        index = node.ChildOfferConditions.Count - 1;
                        node.ChildOfferConditions.Add(new OfferCondition { Id = newId });
                        index = index + 1;
                    }
                    node.ChildOfferConditions[index].ParentId = node.Id;

                    RenderCondition(data, type, isCart, node.ChildOfferConditions[index], hw);

                    responseData = "ok|" + sb.ToString();
                    break;

                case UPDATE_ALL_ANY:
                    switch (data)
                    {
                        case FALSE:
                            node.IsAll = true;
                            node.IsAny = false;
                            break;

                        case TRUE:
                            node.IsAll = false;
                            node.IsAny = true;
                            break;
                    }

                    break;

                case UPDATE_MATCHED:
                    switch (data)
                    {
                        case TRUE:
                            node.Matched = true;
                            break;

                        case FALSE:
                            node.Matched = false;
                            break;
                    }

                    break;

                case UPDATE_ITEM_FOUND:
                    switch (data)
                    {
                        case TRUE:
                            node.ItemFound = true;
                            break;

                        case FALSE:
                            node.ItemFound = false;
                            break;
                    }

                    break;

                case UPDATE_QTY_AMOUNT:
                    switch (data)
                    {
                        case TRUE:
                            var attrSubtotal = _offerService.GetOfferAttribute((int)OfferAttributeType.SUBTOTAL);

                            if (attrSubtotal == null)
                            {
                                responseData = string.Format("Failed to load attribute '{0}:{1}'. Please contact administrator.", "subtotal", OfferAttributeType.SUBTOTAL);
                                _logger.InsertLog(LogLevel.Error, string.Format("Offer attribute could not be loaded. Offer Attribute ID={{{0}}}", OfferAttributeType.SUBTOTAL));
                            }
                            else
                            {
                                node.OfferAttribute = attrSubtotal;
                                node.OfferAttributeId = attrSubtotal.Id;
                                node.IsTotalQty = true;
                                node.IsTotalAmount = false;
                            }
                            break;

                        case FALSE:
                            var attrTotalItemsQty = _offerService.GetOfferAttribute((int)OfferAttributeType.TOTAL_ITEMS_QTY);

                            if (attrTotalItemsQty == null)
                            {
                                responseData = string.Format("Failed to load attribute '{0}:{1}'. Please contact administrator.", "total items quantity", OfferAttributeType.TOTAL_ITEMS_QTY);
                                _logger.InsertLog(LogLevel.Error, string.Format("Offer attribute could not be loaded. Offer Attribute ID={{{0}}}", OfferAttributeType.TOTAL_ITEMS_QTY));
                            }
                            else
                            {
                                node.OfferAttribute = attrTotalItemsQty;
                                node.OfferAttributeId = attrTotalItemsQty.Id;
                                node.IsTotalQty = false;
                                node.IsTotalAmount = true;
                            }
                            break;
                    }

                    break;

                case DELETE_CONDITION:
                    if (root.Id != node.Id)
                        RemoveCondition(root, node.Id);
                    else
                        responseData = "You are not allowed to remove root condition.";
                    break;

                case UPDATE_OPERATOR:
                    var operatorId = Convert.ToInt32(data);
                    OfferOperator op = _offerService.GetOfferOperator(operatorId);

                    if (op == null)
                    {
                        responseData = "There is no such operator. Please contact administrator.";
                        _logger.InsertLog(LogLevel.Error, string.Format("Offer operator could not be loaded. Offer Operator ID={{{0}}}", operatorId));
                    }
                    else
                    {
                        node.OfferOperator = op;
                        node.OfferOperatorId = op.Id;
                    }                    
                    break;

                case UPDATE_OPERAND:
                    node.Operand = data;
                    break;
            }

            response.Clear();
            response.Write(responseData);
            response.End();

            return root;
        }

        private OfferCondition SearchCondition(OfferCondition root, int nodeId)
        {
            if (root.Id == nodeId)
                return root;

            OfferCondition foundCondition = null;

            if (root.ChildOfferConditions != null)
            {
                for (int i = 0; i < root.ChildOfferConditions.Count; i++)
                {
                    foundCondition = SearchCondition(root.ChildOfferConditions[i], nodeId);

                    if (foundCondition != null && foundCondition.Id == nodeId)
                        return foundCondition;
                }
            }

            return null;
        }

        private void RemoveCondition(OfferCondition root, int nodeId)
        {
            if (root.ChildOfferConditions != null)
            {
                for (int i = 0; i < root.ChildOfferConditions.Count; i++)
                {
                    if (root.ChildOfferConditions[i].Id == nodeId)
                    {
                        root.ChildOfferConditions.Remove(root.ChildOfferConditions[i]);
                        break;
                    }
                    else
                        RemoveCondition(root.ChildOfferConditions[i], nodeId);
                }
            }            
        }

        private bool IsProductAttrCombination(OfferCondition condition)
        {
            return condition.ItemFound.HasValue 
                && condition.IsAll.HasValue 
                && condition.IsAny.HasValue 
                && condition.Matched.HasValue;
        }

        private bool IsProductSubselection(OfferCondition condition)
        {
            return condition.IsTotalQty.HasValue 
                && condition.IsTotalAmount.HasValue 
                && (condition.OfferOperator != null) 
                && condition.Operand != null
                && condition.IsAll.HasValue 
                && condition.IsAny.HasValue 
                && condition.Matched.HasValue;
        }
        
        private void RenderCondition(string conditionType, string type, bool isCart, OfferCondition node, HtmlTextWriter writer)
        {
            // conditionType is integer. It could be attribute id too.
            // Thus, we need to cast it to integer first.

            var condition = int.Parse(conditionType);

            switch (condition)
            {
                case OFFER_CONDITIONS_COMBINATION:
                    // Set default value
                    node.IsAll = true;
                    node.IsAny = false;
                    node.Matched = true;

                    RenderCombinationCondition(node.Id.ToString(), type, false, true, false, writer);

                    writer.WriteBeginTag(UL);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    RenderNewSelection(node.Id.ToString(), type, isCart, writer);
                    writer.WriteEndTag(UL);

                    writer.WriteEndTag(LI);
                    break;


                case OFFER_PRODUCT_ATTR_COMBINATION:
                    // Set default value
                    node.ItemFound = true;
                    node.IsAll = true;
                    node.IsAny = false;
                    node.Matched = true;

                    RenderProdAttrCombinationCondition(node.Id.ToString(), type, true, false, false, writer);

                    writer.WriteBeginTag(UL);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    RenderNewSelection(node.Id.ToString(), type, false, writer);
                    writer.WriteEndTag(UL);

                    writer.WriteEndTag(LI);

                    break;

                case OFFER_PRODUCTS_SUBSELECTION:
                    // Set default value
                    node.IsTotalQty = true;
                    node.IsTotalAmount = false;
                    node.OfferAttribute = _offerService.GetOfferAttribute((int)OfferAttributeType.TOTAL_ITEMS_QTY);
                    node.OfferAttributeId = node.OfferAttribute.Id;
                    node.OfferOperator = _offerService.GetOfferOperatorsByAttribute(node.OfferAttribute.Id)[0];
                    node.OfferOperatorId = node.OfferOperator.Id;
                    node.Operand = string.Empty;
                    node.IsAll = true;
                    node.IsAny = false;
                    node.Matched = true;

                    RenderProdSubselectionCondition(node.Id.ToString(),
                                                    type,
                                                    node.OfferOperator.Id,
                                                    string.Empty,
                                                    node.IsTotalQty.Value,
                                                    node.IsTotalAmount.Value,
                                                    false,
                                                    false,
                                                    writer);

                    writer.WriteBeginTag(UL);
                    writer.Write(HtmlTextWriter.TagRightChar);

                    RenderNewSelection(node.Id.ToString(), type, false, writer);

                    writer.WriteEndTag(UL);
                    writer.WriteEndTag(LI);

                    break;

                default:
                    var attributeId = condition;
                    var title = GetTitleByAttributeId(attributeId);
                    var attribute = _offerService.GetOfferAttribute(attributeId);
                    node.OfferAttribute = attribute;
                    node.OfferAttributeId = attribute.Id;

                    var operators = _offerService.GetOfferOperatorsByAttribute(attributeId);

                    if (operators.Count > 0)
                    {
                        node.OfferOperator = operators[0];
                        node.OfferOperatorId = operators[0].Id;
                        RenderOperation(title, node.Id.ToString(), type, operators[0].Id, string.Empty, operators, writer);
                    }

                    break;
            }
        }
        
        private string GetTitleByAttributeId(int offerAttributeId)
        {
            switch ((OfferAttributeType)offerAttributeId)
            {
                case OfferAttributeType.BRAND:
                    return "Brand ";
                case OfferAttributeType.BRAND_CATEGORY:
                    return "Brand Category ";
                case OfferAttributeType.BRAND_CATEGORY_ID:
                    return "Brand Category ID ";
                case OfferAttributeType.BRAND_ID:
                    return "Brand ID ";
                case OfferAttributeType.CATEGORY_ID:
                    return "Category ID ";
                case OfferAttributeType.NAME:
                    return "Name ";
                case OfferAttributeType.PRODUCT_ID:
                    return "Product ID ";
                case OfferAttributeType.SUBTOTAL:
                    return "Total Amount ";
                case OfferAttributeType.TOTAL_ITEMS_QTY:
                    return "Total Quantity ";
                default:
                    return string.Empty;
            }
        }

        #endregion

        #region Nested class

        public enum OfferRenderType
        {
            Cart,
            Catalog,
            Action
        }

        public class OfferConditionCollection
        {
            private Dictionary<string, OfferCondition> _conditions = new Dictionary<string, OfferCondition>();

            public OfferCondition this[int id]
            {
                get
                {
                    if (_conditions.ContainsKey(id.ToString()))
                        return _conditions[id.ToString()];
                    return null;
                }
                set
                {
                    if (_conditions.ContainsKey(id.ToString()))
                        _conditions[id.ToString()] = value;
                    else
                        _conditions.Add(id.ToString(), value);
                }
            }
            public OfferCondition this[string id]
            {
                get
                {
                    if (_conditions.ContainsKey(id))
                        return _conditions[id];
                    return null;
                }
                set
                {
                    if (_conditions.ContainsKey(id))
                        _conditions[id] = value;
                    else
                        _conditions.Add(id, value);
                }
            }
        }
        #endregion
    }
}