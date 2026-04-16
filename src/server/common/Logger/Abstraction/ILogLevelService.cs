using Microsoft.Extensions.Logging;

namespace Logger.Abstraction;

public interface ILogLevelService
{
    LogLevel GetCurrent();

    void Change(LogLevel logLevel);
}
