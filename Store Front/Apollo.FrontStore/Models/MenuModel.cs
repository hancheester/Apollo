using Apollo.Core.Model.OverviewModel;
using Apollo.FrontStore.Models.Category;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models
{
    public class MenuModel
    {
        public IList<CategorySimpleModel> SimpleCategories { get; set; }
        public IList<CategoryOverviewModel> Categories { get; set; }
        public bool IsForSearchResult { get; set; }
        public int CategorySelectedIndex { get; set; }

        public MenuModel()
        {
            SimpleCategories = new List<CategorySimpleModel>();
            Categories = new List<CategoryOverviewModel>();
        }
    }
}