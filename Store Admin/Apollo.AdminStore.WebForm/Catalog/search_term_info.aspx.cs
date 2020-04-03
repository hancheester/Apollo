using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class search_term_info : BasePage
    {
        public ICampaignService CampaignService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadInfo();
        }

        private void LoadInfo()
        {
            var term = CampaignService.GetSearchTerm(QueryId);

            if (term != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", term.Query, QueryId);
                txtQuery.Text = term.Query;
                txtRedirectUrl.Text = term.RedirectUrl;
            }
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            CampaignService.DeleteSearchTerm(QueryId);
            Response.Redirect("/catalog/search_term_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.SearchTermDeleted);
        }

        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var term = CampaignService.GetSearchTerm(QueryId);

            if (term != null)
            {
                term.Query = txtQuery.Text.Trim();
                term.RedirectUrl = txtRedirectUrl.Text.Trim();
                CampaignService.UpdateSearchTerm(term);

                enbNotice.Message = "Search term was updated successfully.";
            }            
        }
    }
}