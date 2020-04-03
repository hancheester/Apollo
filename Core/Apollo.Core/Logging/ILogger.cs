using System;

namespace Apollo.Core.Logging
{
    public interface ILogger
    {
        void InsertLog(LogLevel logLevel, string message, Exception exception = null);
        void InsertLog(LogLevel logLevel, string messageFormat, params object[] args);
    }
}
