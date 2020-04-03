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

namespace Apollo.AdminStore.WebForm.Marketing
{
    /// <summary>
    /// It generates temporary id everytime creating a new offer.
    /// </summary>
    public partial class promo_catalog_offer_new : BasePage
    {
        public IOfferService OfferService { get; set; }
        public OfferUtility OfferUtility { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            rblStatus.SelectedIndex = 1;

            ddlAction.DataSource = OfferService.GetOfferActionAttributes(isCatalog: true, isCart: null);
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

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = QueryTempOfferRuleId;

            if (string.IsNullOrEmpty(id))
            {
                var guid = Guid.NewGuid().ToString();
                Response.Redirect("/marketing/promo_catalog_offer_new.aspx?tempid=" + guid);
            }

            if (OfferUtility.OfferRuleConditions[id] == null)
            {
                OfferUtility.OfferRuleConditions[id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
            }
            
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            OfferUtility.RenderCondition(OfferUtility.OfferRuleConditions[id], hw, OfferUtility.OfferRenderType.Catalog);
            ltlConditions.Text = sb.ToString();
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
                // Instantiate rule object
                var rule = new OfferRule
                {
                    Name = txtRuleName.Text.Trim(),
                    ProceedForNext = Convert.ToBoolean(rblProceed.SelectedValue),
                    IsActive = Convert.ToBoolean(rblStatus.SelectedValue),
                    Priority = Convert.ToInt32(txtPriority.Text.Trim()),
                    Alias = txtRuleAlias.Text.Trim(),
                    ShowOfferTag = chkShowOfferTag.Checked,
                    ShowRRP = chkShowRRP.Checked,
                    ShortDescription = string.Empty,
                    LongDescription = string.Empty,
                    SmallImage = string.Empty,
                    LargeImage = string.Empty,
                    UrlRewrite = urlKey,
                    ShowInOfferPage = false,
                    OfferUrl = string.Empty,
                    PointSpendable = chkPointSpendable.Checked,
                    ShowCountDown = cbShowCountDownTimer.Checked,
                    DisplayOnProductPage = cbDisplayOnProductPage.Checked,
                    OfferTypeId = Convert.ToInt32(ddlOfferTypes.SelectedValue)
                };

                if (!string.IsNullOrEmpty(txtDateFrom.Text.Trim()))
                    rule.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                
                if (!string.IsNullOrEmpty(txtDateTo.Text.Trim()))
                    rule.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                
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
                         
                // Assign action to this rule object's action
                var newAction = new OfferAction
                {
                    Name = ddlAction.SelectedItem.Text,
                    IsCatalog = true,
                    DiscountAmount = Convert.ToDecimal(txtDiscountAmount.Text),
                    OfferActionAttributeId = Convert.ToInt32(ddlAction.SelectedValue)
                };

                // Retrieve size target if any
                string optionOperatorId = null;
                string optionOperator = null;
                string optionOperand = null;

                if (txtOption.Text.Trim() != string.Empty)
                {
                    optionOperand = txtOption.Text.Trim();
                    optionOperatorId = ddlOptionOperator.SelectedValue;
                    optionOperator = OfferService.GetOfferOperator(Convert.ToInt32(optionOperatorId)).Operator;
                }

                if (!string.IsNullOrEmpty(optionOperatorId)) newAction.OptionOperatorId = Convert.ToInt32(optionOperatorId);
                if (!string.IsNullOrEmpty(optionOperand)) newAction.OptionOperand = Convert.ToString(optionOperand);                
                if ((!string.IsNullOrEmpty(optionOperator)) && (!string.IsNullOrEmpty(optionOperatorId)))
                    newAction.OptionOperator = new OfferOperator
                    {
                        Id = Convert.ToInt32(optionOperatorId),
                        Operator = Convert.ToString(optionOperator)
                    };
                      
                rule.Action = newAction;

                var proceed = true;

                // Assign root condition to this rule object's condition
                rule.Condition = OfferUtility.OfferRuleConditions[QueryTempOfferRuleId];

                if (rule.Condition == null)
                {
                    enbNotice.Message = "Offer was failed to create. There is no condition setup for this offer. It could be due to session lost. Please try to create again.";
                    proceed = false;
                }

                if (proceed)
                {
                    // Save into database
                    var id = OfferService.ProcessOfferInsertion(rule);

                    // Reset value
                    ResetCatalogOfferSessions();

                    // Redirect to info page
                    Response.Redirect("/marketing/promo_catalog_offer_info.aspx?offerruleid=" + id + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OfferCreated);
                }
            }
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("/marketing/promo_catalog_offer_new.aspx");
        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            ResetCatalogOfferSessions();
            Response.Redirect("/marketing/promo_catalog_offer_default.aspx");
        }

        private void ResetCatalogOfferSessions()
        {
            OfferUtility.OfferRuleConditions[QueryTempOfferRuleId] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
        }
    }
}