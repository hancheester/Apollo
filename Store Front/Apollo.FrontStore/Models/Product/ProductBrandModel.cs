using Apollo.FrontStore.Models.Media;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductBrandModel
    {
        public PictureModel Picture { get; set; }
        public string BrandUrl { get; set; }
        public bool Visible { get; set; }
        public string Name { get; set; }
    }
}