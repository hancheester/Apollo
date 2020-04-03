using Apollo.Core.Caching;
using Apollo.Core.Model.Entity;
using Apollo.Core.NHunspell;
using Apollo.Core.Services.Interfaces;
using NHunspell;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Apollo.Core.Services.Common
{
    public class SpellCheckerService : ISpellCheckerService
    {
        private const string BRAND_ALL_KEY = "brand.all";
        private const string CUSTOM_DICTIONARY_ALL_KEY = "customdictionary.all";

        private Soundex _soundex;
        private Hunspell _hunspell;
        private readonly NHunspellSettings _nhunspellSettings;
        private readonly IBrandService _brandService;
        private readonly IUtilityService _utilityService;
        private readonly ICacheManager _cacheManager;

        public SpellCheckerService(
            IBrandService brandService,
            IUtilityService utilityService,
            ICacheManager cacheManager,
            NHunspellSettings nhunspellSettings)
        {
            _nhunspellSettings = nhunspellSettings;
            _brandService = brandService;
            _utilityService = utilityService;
            _cacheManager = cacheManager;
            _hunspell = BuildHunspell(initialize: true);

            _soundex = new Soundex();
        }

        public bool Spell(string word)
        {
            return _hunspell.Spell(word);
        }

        public IList<string> Suggest(string word)
        {
            var hunspell = BuildHunspell();
            var keywords = hunspell.Suggest(word);
            var soundexTarget = _soundex.For(word);
            var soundexList = new List<string>();

            // Search for soundex values
            foreach (var keyword in keywords)
            {
                var soundexCurrent = _soundex.For(keyword);
                if (soundexTarget.CompareTo(soundexCurrent) == 0)
                    soundexList.Add(keyword);
            }

            if (soundexList.Count > 0)
            {
                // Search for brand name
                foreach (var soundexItem in soundexList)
                {
                    var foundBrand = _brandService.GetBrandByName(soundexItem);
                    if (foundBrand != null)
                    {
                        return new List<string> { foundBrand.Name };
                    }
                }
            }

            return soundexList.Count == 0 ? keywords : soundexList;
        }

        private Hunspell BuildHunspell(bool initialize = false)
        {
            // If cache item is not found, then reload hunspell object again.
            if (initialize || _cacheManager.IsSet(BRAND_ALL_KEY) == false || _cacheManager.IsSet(CUSTOM_DICTIONARY_ALL_KEY) == false)
            {
                var affFilePath = _nhunspellSettings.NHunspellResourceAffFilePath;
                var dictFilePath = _nhunspellSettings.NHunspellResourceDictFilePath;
                _hunspell = new Hunspell(affFilePath, dictFilePath);

                var brands = _brandService.GetBrandList();
                foreach (var brand in brands)
                {
                    _hunspell.Add(brand.Name);

                    var cleanedName = RemoveDiacritics(brand.Name);
                    if (cleanedName != brand.Name)
                        _hunspell.Add(cleanedName);
                }

                var dictionaries = GetAllCustomDictionary();
                foreach (var item in dictionaries)
                {
                    _hunspell.Add(item.Word);
                    var cleanedName = RemoveDiacritics(item.Word);
                    if (cleanedName != item.Word)
                        _hunspell.Add(cleanedName);
                }
            }

            return _hunspell;
        }

        private IList<CustomDictionary> GetAllCustomDictionary()
        {
            var dictionaries = _cacheManager.Get(CUSTOM_DICTIONARY_ALL_KEY, delegate ()
            {
                return _utilityService.GetAllCustomDictionary();
            });

            return dictionaries;
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
