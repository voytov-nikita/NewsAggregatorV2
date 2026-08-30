using Microsoft.AspNetCore.Identity;

namespace AuthService.DAL.PostgreSql.Entities;

/// <summary>
/// Guid key rather than Identity's default string: the user id crosses service boundaries
/// (NewsService stores it on comments and votes), where a native uuid column beats a
/// 450-char varchar holding a GUID's string form.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }
}
