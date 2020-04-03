namespace Apollo.Core.Logging
{
    public interface ILogBuilder
    {
        ILogger CreateLogger(string loggerName);
    }
}
