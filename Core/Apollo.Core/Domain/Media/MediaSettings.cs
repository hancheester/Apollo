using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Media
{
    public class MediaSettings : ISettings
    {
        public string BrandMediaLocalPath { get; set; }
        public string BrandMediaPath { get; set; }
        public string ProductColourLocalPath { get; set; }
        public string ProductColourPath { get; set; }
        public string ProductMediaLocalPath { get; set; }
        public string ProductMediaPath { get; set; }
        public string CategoryMediaPath { get; set; }
        public string CategoryMediaLocalPath { get; set; }
        public string OfferMediaPath { get; set; }
        public string OfferMediaLocalPath { get; set; }
        public string MediumBannerPath { get; set; }
        public string MediumBannerLocalPath { get; set; }
        public string MiniBannerPath { get; set; }
        public string MiniBannerLocalPath { get; set; }
        public string LargeBannerPath { get; set; }
        public string LargeBannerLocalPath { get; set; }
        public string OfferBannerPath { get; set; }
        public string OfferBannerLocalPath { get; set; }
        public string NoImagePath { get; set; }
        public string NoImageLocalPath { get; set; }
        public string LargeLogoLink { get; set; }
    }
}
