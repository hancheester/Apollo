using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Caching;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_cache_default : BasePage, ICallbackEventHandler
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }
        public ICacheManager CacheManager { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<CacheSettings>();
                txtStoreFrontRefreshCacheLink.Text = setting.StoreFrontRefreshCacheLink;
                txtStoreFrontGetPerfDataLink.Text = setting.StoreFrontGetPerfDataLink;
                txtStoreFrontGetCacheKeysLink.Text = setting.StoreFrontGetCacheKeysLink;
                txtStoreFrontToken.Text = setting.StoreFrontToken;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<CacheSettings>();
            setting.StoreFrontRefreshCacheLink = txtStoreFrontRefreshCacheLink.Text.Trim();
            setting.StoreFrontGetPerfDataLink = txtStoreFrontGetPerfDataLink.Text.Trim();
            setting.StoreFrontGetCacheKeysLink = txtStoreFrontGetCacheKeysLink.Text.Trim();
            setting.StoreFrontToken = txtStoreFrontToken.Text.Trim();

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Cache settings were updated successfully.";
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Setting);

            if (result)
                enbNotice.Message = "All settings related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        private string _message;

        public void RaiseCallbackEvent(string eventArgument)
        {
            string type = string.Format("{0}_manager", eventArgument);
            IDictionary<string, string> data = null;
            IList<string> keys = null;
            switch (type)
            {
                case "admin_memory_cache_manager":
                    data = CacheManager.GetPerformanceData();
                    break;
                case "admin_memory_keys_manager":
                    keys = CacheManager.GetCacheKeys();
                    break;
                case "store_memory_cache_manager":
                    data = UtilityService.GetCachePerformanceData(type);
                    break;
                case "store_memory_keys_manager":
                    keys = UtilityService.GetCacheKeys(type);
                    break;
                case "service_memory_cache_manager":
                    data = UtilityService.GetCachePerformanceData(type);
                    break;
                case "service_memory_keys_manager":
                    keys = UtilityService.GetCacheKeys(type);
                    break;
                case "service_dache_cache_manager":
                    data = UtilityService.GetCachePerformanceData(type);
                    break;
                case "service_dache_keys_manager":
                    keys = UtilityService.GetCacheKeys(type);
                    break;
                default:
                    break;
            }
            
            if (data != null)
            {
                StringBuilder sb = new StringBuilder();                
                foreach (var item in data)
                {
                    sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", item.Key, item.Value);
                }

                _message = sb.ToString();
            }

            if (keys != null)
            {
                keys = keys.OrderBy(x => x).ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var key in keys)
                {
                    sb.AppendFormat("<tr><td>{0}</td></tr>", key);
                }

                _message = sb.ToString();
            }
        }

        public string GetCallbackResult()
        {
            return _message;
        }
    }
}