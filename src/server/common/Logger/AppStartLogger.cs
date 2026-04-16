using Serilog;

namespace Logger;

/// <summary>
/// Writes structured log entries before the DI container is built. Intended for
/// startup/shutdown events and fatal host-termination diagnostics.
/// </summary>
public static class AppStartLogger
{
    public static void Fatal(Exception exc, string message)
    {
        Log.Logger.Fatal(exc, message);
    }

    public static void Warning(Exception exc, string message)
    {
        Log.Logger.Warning(exc, message);
    }

    public static void Info(string message)
    {
        Log.Logger.Information(message);
    }

    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
