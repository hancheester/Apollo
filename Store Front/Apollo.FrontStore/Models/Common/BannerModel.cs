using Apollo.FrontStore.Models.Media;
using Apollo.Web.Framework.Mvc;

namespace Apollo.FrontStore.Models.Common
{
    public class BannerModel : BaseEntityModel
    {
        public PictureModel Picture { get; set; }
        public string Link { get; set; }
        
        public BannerModel()
        {
            Picture = new PictureModel();
        }
    }
}