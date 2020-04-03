using Apollo.Core.Domain.Cms;
using Apollo.Core.Logging;
using Apollo.Core.Plugins;
using Apollo.Core.Services.Interfaces.Cms;
using Apollo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Cms
{
    public class WidgetService : BaseRepository, IWidgetService
    {
        #region Fields

        private readonly ILogger logger;
        private readonly IPluginFinder _pluginFinder;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor
        
        public WidgetService(
            ILogBuilder logBuilder,
            IPluginFinder pluginFinder, 
            WidgetSettings widgetSettings)
        {
            _pluginFinder = pluginFinder;
            _widgetSettings = widgetSettings;
            logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        public IList<IWidgetPlugin> LoadActiveWidgets(int storeId = 0)
        {
            return LoadAllWidgets(storeId)
                   .Where(x => _widgetSettings.ActiveWidgetSystemNames.Contains(x.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        public IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string widgetZone, int storeId = 0)
        {
            if (string.IsNullOrWhiteSpace(widgetZone))
                return new List<IWidgetPlugin>();

            var widgets = LoadActiveWidgets(storeId)
                   .Where(x => x.GetWidgetZones().Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();

            return widgets;
        }

        /// <summary>
        /// Load widget by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
        public IWidgetPlugin LoadWidgetBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IWidgetPlugin>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IWidgetPlugin>();

            return null;
        }

        /// <summary>
        /// Load all widgets
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        public IList<IWidgetPlugin> LoadAllWidgets(int storeId = 0)
        {
            return _pluginFinder.GetPlugins<IWidgetPlugin>(storeId: storeId).ToList();
        }

        #endregion
    }
}
