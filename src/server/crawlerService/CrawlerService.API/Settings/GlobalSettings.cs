using Common.Auth.Settings;

namespace CrawlerService.API.Settings;

public class GlobalSettings
{
    public AuthSettings Auth { get; set; } = null!;
}
