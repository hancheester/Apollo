using Apollo.Core.Caching;
using Apollo.Core.Configuration;
using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Apollo.Core.Services.Configuration
{
    public class SettingService : BaseRepository, ISettingService
    {
        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int StoreId { get; set; }            
        }

        #endregion

        #region Fields
        
        private readonly IRepository<Setting> _settingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public SettingService(
            IRepository<Setting> settingRepository,
            ILogBuilder logBuilder,
            ICacheManager cacheManager)
        {
            _settingRepository = settingRepository;
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        public Setting GetSettingById(int settingId)
        {
            return _settingRepository.Return(settingId);
        }

        public void DeleteSetting(int settingId)
        {
            _settingRepository.Delete(settingId);
            _cacheManager.RemoveByPattern(CacheKey.SETTING_PATTERN_KEY);
        }

        public void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null) throw new ArgumentNullException("setting");

            _settingRepository.Create(setting);
            
            if (clearCache) _cacheManager.RemoveByPattern(CacheKey.SETTING_PATTERN_KEY);
        }

        public void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null) throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);

            if (clearCache) _cacheManager.RemoveByPattern(CacheKey.SETTING_PATTERN_KEY);
        }

        public T LoadSetting<T>(int storeId = 0) where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }

        public ISettings LoadSetting(string name, int storeId = 0)
        {
            Type type = Type.GetType(name);
            var settings = Activator.CreateInstance(type);
            
            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return (ISettings)settings;
        }
        
        public void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetApolloCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value, storeId, false);
                else
                    SetSetting(key, "", storeId, false);
            }

            //and now clear cache
            _cacheManager.RemoveByPattern(CacheKey.SETTING_PATTERN_KEY);
        }

        public void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            if (key == null) throw new ArgumentNullException("key");

            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetApolloCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;

            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };

                InsertSetting(setting, clearCache);
            }
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key)) return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

                //load shared value?
                if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }
        
        public void DeleteSetting(Setting setting)
        {
            if (setting == null) throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(CacheKey.SETTING_PATTERN_KEY);
        }

        public void DeleteSetting<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = GetAllSettings();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                DeleteSetting(setting);
        }

        public void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
        {
            string key = settings.GetSettingKey(keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                DeleteSetting(setting);
            }
        }

        public IList<Setting> GetAllSettings()
        {
            var query = _settingRepository.Table.OrderBy(x => x.Name).ThenBy(x => x.StoreId);
            var settings = query.ToList();
            return settings;
        }

        protected IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            //cache
            string key = string.Format(CacheKey.SETTING_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = _settingRepository.TableNoTracking
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.StoreId);

                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        StoreId = s.StoreId
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }
    }
}
