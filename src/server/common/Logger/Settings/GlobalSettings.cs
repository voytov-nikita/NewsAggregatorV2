using Serilog.Events;

namespace Logger.Settings;

public class GlobalSettings
{
    public SerilogSettings Serilog { get; set; } = null!;
}

public class SerilogSettings
{
    public MinimumLevelSettings MinimumLevel { get; set; } = null!;
}

public class MinimumLevelSettings
{
    public LogEventLevel Default { get; set; }
}
