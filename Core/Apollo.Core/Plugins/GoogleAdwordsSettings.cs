using Apollo.Core.Configuration;

namespace Apollo.Core.Plugins
{
    public class GoogleAdwordsSettings : ISettings
    {
        public string GoogleConversionId { get; set; }
        public string GoogleConversionLabel { get; set; }
        public string TrackingScript { get; set; }
    }
}
