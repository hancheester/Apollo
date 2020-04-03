using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class custom_dict_new : BasePage
    {
        public ISearchService SearchService { get; set; }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var newItem = new CustomDictionary();
            newItem.Word = txtWord.Text.Trim();

            int id = SearchService.InsertCustomDictionary(newItem);

            Response.Redirect("/catalog/custom_dict_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.CustomDictionaryCreated + "&" + QueryKey.ID + "=" + id.ToString());
        }
    }
}