namespace AuthService.API.Settings;

public class ConnectionStringSettings
{
    public string PostgreSql { get; set; } = null!;

    public int CommandTimeout { get; set; }
}
