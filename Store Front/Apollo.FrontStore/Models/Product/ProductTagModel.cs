using Apollo.Web.Framework.Mvc;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductTagModel : BaseEntityModel
    {
        public string Tag { get; set; }
        public string Description { get; set; }
    }
}