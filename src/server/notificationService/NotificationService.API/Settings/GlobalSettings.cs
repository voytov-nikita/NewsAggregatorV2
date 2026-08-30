using Common.Auth.Settings;

namespace NotificationService.Settings;

public class GlobalSettings
{
    public AuthSettings Auth { get; set; } = null!;
}
