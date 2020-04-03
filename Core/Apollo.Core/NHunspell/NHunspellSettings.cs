using Apollo.Core.Configuration;

namespace Apollo.Core.NHunspell
{
    public class NHunspellSettings : ISettings
    {
        public string NHunspellResourceAffFilePath { get; set; }
        public string NHunspellResourceDictFilePath { get; set; }
    }
}
