using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class custom_dict_info : BasePage
    {
        public ISearchService SearchService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadInfo();
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            SearchService.DeleteCustomDictionary(QueryId);
            Response.Redirect("/catalog/custom_dict_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.CustomDictionaryDeleted);
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var term = SearchService.GetCustomDictionary(QueryId);

            if (term != null)
            {
                term.Word = txtWord.Text.Trim();
                SearchService.UpdateCustomDictionary(term);

                enbNotice.Message = "Custom dictionary word was updated successfully.";
            }
        }

        private void LoadInfo()
        {
            var term = SearchService.GetCustomDictionary(QueryId);

            if (term != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", term.Word, QueryId);
                txtWord.Text = term.Word;
            }
        }
    }
}