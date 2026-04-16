using Logger.Abstraction;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Logger;

internal class LogLevelService : ILogLevelService
{
    private readonly LoggingLevelSwitch _levelSwitch;

    public LogLevelService(LoggingLevelSwitch levelSwitch)
    {
        _levelSwitch = levelSwitch;
    }

    public LogLevel GetCurrent()
    {
        return FromSerilogToMicrosoft(_levelSwitch.MinimumLevel);
    }

    public void Change(LogLevel logLevel)
    {
        _levelSwitch.MinimumLevel = FromMicrosoftToSerilog(logLevel);
    }

    private static LogLevel FromSerilogToMicrosoft(LogEventLevel level) => level switch
    {
        LogEventLevel.Verbose => LogLevel.Trace,
        LogEventLevel.Debug => LogLevel.Debug,
        LogEventLevel.Information => LogLevel.Information,
        LogEventLevel.Warning => LogLevel.Warning,
        LogEventLevel.Error => LogLevel.Error,
        LogEventLevel.Fatal => LogLevel.Critical,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };

    private static LogEventLevel FromMicrosoftToSerilog(LogLevel level) => level switch
    {
        LogLevel.Trace => LogEventLevel.Verbose,
        LogLevel.Debug => LogEventLevel.Debug,
        LogLevel.Information => LogEventLevel.Information,
        LogLevel.Warning => LogEventLevel.Warning,
        LogLevel.Error => LogEventLevel.Error,
        LogLevel.Critical => LogEventLevel.Fatal,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };
}
