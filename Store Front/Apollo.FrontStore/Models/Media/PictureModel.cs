using Apollo.Web.Framework.Mvc;

namespace Apollo.FrontStore.Models.Media
{
    public class PictureModel : BaseEntityModel
    {
        public string ImageUrl { get; set; }
        public string FullSizeImageUrl { get; set; }
        public string Title { get; set; }
        public string AlternateText { get; set; }
    }
}