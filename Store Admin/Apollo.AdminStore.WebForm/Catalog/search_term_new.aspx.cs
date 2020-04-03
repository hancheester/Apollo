using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class search_term_new : BasePage
    {
        public ICampaignService CampaignService { get; set; }
        
        protected void lbSave_Click(object sender, EventArgs e)
        {
            SearchTerm newSearchTerm = new SearchTerm();
            newSearchTerm.Query = txtQuery.Text.Trim();
            newSearchTerm.RedirectUrl = txtRedirectUrl.Text.Trim();

            int id = CampaignService.InsertSearchTerm(newSearchTerm);

            Response.Redirect("/catalog/search_term_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.SearchTermCreated + "&" + QueryKey.ID + "=" + id.ToString());
        }
    }
}