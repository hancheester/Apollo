using Apollo.Core.Logging;
using log4net;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class LogBuilder : ILogBuilder
    {
        public ILogger CreateLogger(string loggerName)
        {
            return new Logger(LogManager.GetLogger(loggerName));
        }
    }
}