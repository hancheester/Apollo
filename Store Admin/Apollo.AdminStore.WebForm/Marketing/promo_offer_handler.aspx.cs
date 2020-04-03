using Apollo.AdminStore.WebForm.Classes;
using log4net;
using System;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class promo_offer_handler : BasePage
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(promo_offer_handler).FullName);

        public OfferUtility OfferUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var treeId = Request.Form["treeId"];
            var type = Request.Form["type"];

            if (string.IsNullOrEmpty(treeId))
            {
                _logger.Error("Form variable 'treeId' is not found.");
                Response.Write("Form variable 'treeId' is not found. Please contact administrator.");
                Response.End();
            }

            switch (type)
            {
                case "action":
                    if (OfferUtility.OfferActionConditions[treeId] == null)
                    {
                        _logger.ErrorFormat("Action condition tree could not be loaded. treeId={{{0}}}", treeId);
                        Response.Write(string.Format("Action condition tree could not be loaded. Please try to refresh the page. treeId={{{0}}}", treeId));
                        Response.End();
                    }

                    OfferUtility.ProcessActionCondition(Request, 
                                                        Response,
                                                        treeId,
                                                        type);
                    break;
                default:
                case "cart":
                case "catalog":
                    if (OfferUtility.OfferRuleConditions[treeId] == null)
                    {
                        _logger.ErrorFormat("Offer condition tree could not be loaded. treeId={{{0}}}", treeId);
                        Response.Write(string.Format("Offer condition tree could not be loaded. Please try to refresh the page. treeId={{{0}}}", treeId));
                        Response.End();
                    }

                    OfferUtility.ProcessOfferRuleCondition(Request, 
                                                           Response,
                                                           treeId,
                                                           type);
                    break;
            }

        }
    }
}