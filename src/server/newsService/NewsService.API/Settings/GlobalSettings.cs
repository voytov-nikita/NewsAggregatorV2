using Common.Auth.Settings;

namespace NewsService.API.Settings;

public class GlobalSettings
{
	public ConnectionStringSettings ConnectionStrings { get; set; } = null!;

	public AuthSettings Auth { get; set; } = null!;
}
