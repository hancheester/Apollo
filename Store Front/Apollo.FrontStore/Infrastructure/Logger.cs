using Apollo.Core.Logging;
using log4net;
using System;

namespace Apollo.FrontStore.Infrastructure
{
    public class Logger : ILogger
    {
        private ILog _logger;

        public Logger(ILog logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public void InsertLog(LogLevel logLevel, string messageFormat, params object[] args)
        {
            InsertLog(logLevel, string.Format(messageFormat, args));
        }

        public void InsertLog(LogLevel logLevel, string message, Exception exception = null)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Information:
                    _logger.Info(message);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, exception);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message);
                    break;
                default:
                    break;
            }
        }
    }
}