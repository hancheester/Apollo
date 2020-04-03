using Apollo.FrontStore.Models.Media;

namespace Apollo.FrontStore.Models.Category
{
    public class CategorySimpleModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public PictureModel Picture { get; set; }

        public CategorySimpleModel()
        {
            Picture = new PictureModel();
        }
    }
}