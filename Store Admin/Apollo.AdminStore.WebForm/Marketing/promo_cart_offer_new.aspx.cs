using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    /// <summary>
    /// It generates temporary id everytime creating a new offer.
    /// </summary>
    public partial class promo_cart_offer_new : BasePage
    {
        public IOfferService OfferService { get; set; }
        public OfferUtility OfferUtility { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            rblStatus.SelectedIndex = 1;
            
            ddlAction.DataSource = OfferService.GetOfferActionAttributes(isCatalog: null, isCart: true);
            ddlAction.DataTextField = "Name";
            ddlAction.DataValueField = "Id";
            ddlAction.DataBind();

            ddlOptionOperator.DataSource = OfferService.GetOfferOperatorsByAttribute((int)OfferAttributeType.OPTION);
            ddlOptionOperator.DataTextField = "Operator";
            ddlOptionOperator.DataValueField = "Id";
            ddlOptionOperator.DataBind();

            var types = OfferService.GetOfferTypes().ToList();
            types.Insert(0, new OfferType { Type = AppConstant.DEFAULT_SELECT });
            ddlOfferTypes.DataTextField = "Type";
            ddlOfferTypes.DataValueField = "Id";
            ddlOfferTypes.DataSource = types;
            ddlOfferTypes.DataBind();

            if (IsFireFoxBrowser)
                txtDescription.Visible = false;
            else
                ftbDesc.Visible = false;

            base.OnInit(e);
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = QueryTempOfferRuleId;

            if (string.IsNullOrEmpty(id))
            {
                var guid = Guid.NewGuid().ToString();
                Response.Redirect("/marketing/promo_cart_offer_new.aspx?tempid=" + guid);
            }

            if (OfferUtility.OfferRuleConditions[id] == null)
            {
                OfferUtility.OfferRuleConditions[id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
                OfferUtility.OfferActionConditions[id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
            }

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            OfferUtility.RenderCondition(OfferUtility.OfferRuleConditions[id], hw, OfferUtility.OfferRenderType.Cart);
            ltlConditions.Text = sb.ToString();

            StringBuilder sb2 = new StringBuilder();
            StringWriter tw2 = new StringWriter(sb2);
            HtmlTextWriter hw2 = new HtmlTextWriter(tw2);
            OfferUtility.RenderCondition(OfferUtility.OfferActionConditions[id], hw2, OfferUtility.OfferRenderType.Action);
            ltlActionConditions.Text = sb2.ToString();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            string urlKey = txtUrlKey.Text.Trim();

            // If urlKey is empty, regenerate with given name
            if (urlKey == string.Empty)
            {
                urlKey = AdminStoreUtility.GetFriendlyUrlKey(txtRuleAlias.Text.Trim());
                txtUrlKey.Text = urlKey;
            }

            // Make sure urlKey is unique
            if (((OfferService.GetOfferRuleByUrlKey(urlKey) != null) && (OfferService.GetOfferRuleByUrlKey(urlKey).Id != QueryOfferRuleId)))
            {
                enbNotice.Message = "Offer was failed to create. URL key is not unique.";
            }
            else
            {
                string htmlMsg;

                if (ftbDesc.Visible)
                    htmlMsg = AdminStoreUtility.CleanFtbOutput(ftbDesc.Text);
                else
                    htmlMsg = txtDescription.Text.Trim();

                var rule = new OfferRule
                {
                    Name = txtRuleName.Text.Trim(),
                    ProceedForNext = Convert.ToBoolean(rblProceed.SelectedValue),
                    IsActive = Convert.ToBoolean(rblStatus.SelectedValue),
                    IsCart = true,
                    PromoCode = txtPromoCode.Text.Trim(),
                    UsesPerCustomer = Convert.ToInt32(txtUsesPerCust.Text.Trim()),
                    Priority = Convert.ToInt32(txtPriority.Text.Trim()),
                    HtmlMessage = htmlMsg,
                    OfferedItemIncluded = cbOfferedItemIncluded.Checked,
                    Alias = txtRuleAlias.Text.Trim(),
                    ShowOfferTag = false,
                    ShowRRP = false,
                    ShortDescription = string.Empty,
                    LongDescription = string.Empty,
                    SmallImage = string.Empty,
                    LargeImage = string.Empty,
                    UrlRewrite = urlKey,
                    ShowInOfferPage = false,
                    OfferUrl = string.Empty,
                    PointSpendable = chkPointSpendable.Checked,
                    UseInitialPrice = cbUseInitialPrice.Checked,
                    NewCustomerOnly = cbNewCustomerOnly.Checked,
                    ShowCountDown = cbShowCountDownTimer.Checked,
                    DisplayOnProductPage = cbDisplayOnProductPage.Checked,
                    OfferTypeId = Convert.ToInt32(ddlOfferTypes.SelectedValue)
                };
                
                if (ddlRelatedTypes.SelectedValue != string.Empty)
                {
                    switch (ddlRelatedTypes.SelectedValue)
                    {
                        case "products":
                            rule.RelatedProducts = txtRelatedItems.Text.ToString();
                            break;

                        case "brands":
                            rule.RelatedBrands = txtRelatedItems.Text.ToString();
                            break;

                        case "category":
                            rule.RelatedCategory = txtRelatedItems.Text.ToString();
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(txtDateFrom.Text.Trim()))
                    rule.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);

                if (!string.IsNullOrEmpty(txtDateTo.Text.Trim()))
                    rule.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                
                object freeProductItself = DBNull.Value;
                object freeProductId = DBNull.Value;
                object freeProductPriceId = DBNull.Value;
                object freeProductQty = DBNull.Value;

                // Determine if there is any free item settings
                if (rbFreeItself.Checked || rbFreeItem.Checked)
                {
                    freeProductItself = rbFreeItself.Checked;

                    if (rbFreeItem.Checked)
                    {
                        freeProductId = txtFreeProductId.Text.Trim();
                        freeProductPriceId = txtFreeProductPriceId.Text.Trim();
                        freeProductQty = txtFreeQuantity.Text.Trim();
                    }
                }

                object discountQtyStep = DBNull.Value;
                object minimumAmount = DBNull.Value;
                object discountAmount = DBNull.Value;
                int rewardPoint = 0;

                if (txtDiscountQtyStep.Text.Trim() != string.Empty)
                    discountQtyStep = txtDiscountQtyStep.Text.Trim();

                if (txtMinimumAmount.Text.Trim() != string.Empty)
                    minimumAmount = txtMinimumAmount.Text.Trim();

                if (txtDiscountAmount.Text.Trim() != string.Empty)
                    discountAmount = txtDiscountAmount.Text.Trim();

                if (txtRewardPoint.Text.Trim() != string.Empty)
                    rewardPoint = Convert.ToInt32(txtRewardPoint.Text.Trim());

                // Retrieve option target if any
                object optionOperatorId = DBNull.Value;
                object optionOperator = DBNull.Value;
                object optionOperand = DBNull.Value;

                if (txtOption.Text.Trim() != string.Empty)
                {
                    optionOperand = txtOption.Text.Trim();
                    optionOperatorId = ddlOptionOperator.SelectedValue;
                    optionOperator = OfferService.GetOfferOperator(Convert.ToInt32(optionOperatorId)).Operator;
                }

                // Assign action to this rule object's action
                var newAction = new OfferAction();

                if (discountAmount != DBNull.Value) newAction.DiscountAmount = Convert.ToDecimal(discountAmount);
                if (freeProductItself != DBNull.Value) newAction.FreeProductItself = Convert.ToBoolean(freeProductItself);
                if (freeProductId != DBNull.Value) newAction.FreeProductId = Convert.ToInt32(freeProductId);
                if (freeProductPriceId != DBNull.Value) newAction.FreeProductPriceId = Convert.ToInt32(freeProductPriceId);
                if (freeProductQty != DBNull.Value) newAction.FreeProductQty = Convert.ToInt32(freeProductQty);
                newAction.OfferActionAttributeId = Convert.ToInt32(ddlAction.SelectedValue);
                newAction.Name = ddlAction.SelectedItem.Text;
                newAction.IsCart = true;
                if (discountQtyStep != DBNull.Value) newAction.DiscountQtyStep = Convert.ToInt32(discountQtyStep);
                if (minimumAmount != DBNull.Value) newAction.MinimumAmount = Convert.ToDecimal(minimumAmount);
                newAction.RewardPoint = rewardPoint;
                if (optionOperatorId != DBNull.Value) newAction.OptionOperatorId = Convert.ToInt32(optionOperatorId);
                if (optionOperand != DBNull.Value) newAction.OptionOperand = Convert.ToString(optionOperand);

                if ((optionOperator != DBNull.Value) && (optionOperatorId != DBNull.Value))
                    newAction.OptionOperator = new OfferOperator
                    {
                        Id = Convert.ToInt32(optionOperatorId),
                        Operator = Convert.ToString(optionOperator)
                    };

                newAction.XValue = Convert.ToInt32(txtXValue.Text.Trim());
                newAction.YValue = Convert.ToDecimal(txtYValue.Text.Trim());

                rule.Action = newAction;

                var proceed = true;

                // Assign root condition to this rule object's condition
                rule.Condition = OfferUtility.OfferRuleConditions[QueryTempOfferRuleId];

                if (rule.Condition == null)
                {
                    enbNotice.Message = "Offer was failed to create. There is no condition setup for this offer. It could be due to session lost. Please try to create again.";
                    proceed = false;
                }

                // Assign root condition to this action object's condition
                rule.Action.Condition = OfferUtility.OfferActionConditions[QueryTempOfferRuleId];

                if (rule.Action.Condition == null)
                {
                    enbNotice.Message = "Offer was failed to create. There is no <u>action</u> condition setup for this offer. It could be due to session lost. Please try to create again.";
                    proceed = false;
                }

                if (proceed)
                {
                    // Save into database
                    var id = OfferService.ProcessOfferInsertion(rule);

                    ResetCartOfferSessions();

                    // Redirect to info page
                    Response.Redirect("/marketing/promo_cart_offer_info.aspx?offerruleid=" + id + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OfferCreated);
                }
            }
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            ResetCartOfferSessions();
            Response.Redirect("/marketing/promo_cart_offer_new.aspx");
        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            ResetCartOfferSessions();
            Response.Redirect("/marketing/promo_cart_offer_default.aspx");
        }

        private void ResetCartOfferSessions()
        {
            var id = QueryTempOfferRuleId;
            OfferUtility.OfferRuleConditions[id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
            OfferUtility.OfferActionConditions[id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
        }
    }
}