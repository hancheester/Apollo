﻿using Apollo.Dache.Client.Serialization;
using Apollo.Dache.Core.Logging;
using System;

namespace Apollo.Dache.Client.Configuration
{
    /// <summary>
    /// Loads custom types from configuration.
    /// </summary>
    internal static class CustomTypesLoader
    {
        /// <summary>
        /// Loads a custom logger. If one was not specified, or the loading fails, loads the default logger.
        /// </summary>
        /// <param name="configuration">The cache client configuration.</param>
        /// <returns>The logger.</returns>
        public static ILogger LoadLogger(CacheClientConfigurationSection configuration)
        {
            var defaultLogger = new FileLogger();

            // Configure custom logging
            try
            {
                var customLoggerTypeString = configuration.CustomLogger.Type;
                // Check for custom logger
                if (string.IsNullOrWhiteSpace(customLoggerTypeString))
                {
                    // No custom logging
                    return defaultLogger;
                }

                // Have a custom logger, attempt to load it and confirm it
                var customLoggerType = Type.GetType(customLoggerTypeString);
                // Verify that it implements our ILogger interface
                if (customLoggerType != null && typeof(ILogger).IsAssignableFrom(customLoggerType))
                {
                    return (ILogger)Activator.CreateInstance(customLoggerType);
                }
            }
            catch (Exception ex)
            {
                defaultLogger.Error(ex);
            }

            return defaultLogger;
        }

        /// <summary>
        /// Loads a custom serializer. If one was not specified, or the loading fails, loads the default serializer.
        /// </summary>
        /// <param name="configuration">The cache client configuration.</param>
        /// <returns>The serializer.</returns>
        public static IBinarySerializer LoadSerializer(CacheClientConfigurationSection configuration)
        {
            var defaultSerializer = new BinarySerializer();

            // Configure custom serializer
            try
            {
                var customSerializerTypeString = configuration.CustomSerializer.Type;
                // Check for custom serializer
                if (string.IsNullOrWhiteSpace(customSerializerTypeString))
                {
                    // No custom serializer
                    return defaultSerializer;
                }

                // Have a custom serializer, attempt to load it and confirm it
                var customSerializerType = Type.GetType(customSerializerTypeString);
                // Verify that it implements our IBinarySerializer interface
                if (customSerializerType != null && typeof(IBinarySerializer).IsAssignableFrom(customSerializerType))
                {
                    return (IBinarySerializer)Activator.CreateInstance(customSerializerType);
                }
                if (customSerializerType == null) { throw new Exception("custom serializer is null"); }
                if (!customSerializerType.IsAssignableFrom(typeof(IBinarySerializer))) { throw new Exception("custom serializer is not of type IBinarySerializer"); }
            }
            catch (Exception ex)
            {
                // Custom serializer load failed - no custom serialization
                LoadLogger(configuration).Error(ex);
            }

            return defaultSerializer;
        }
    }
}
