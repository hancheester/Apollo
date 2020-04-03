using Apollo.Core.Configuration;

namespace Apollo.Core.Services.Interfaces
{
    public interface ISettingService
    {
        T LoadSetting<T>(int storeId = 0) where T : ISettings, new();
        void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new();
        void DeleteSetting<T>() where T : ISettings, new();
    }
}
