namespace Common.Auth.Services;

/// <summary>
/// Read-only view over the claims of the caller. Abstracts claims, not HTTP, so BLL services
/// can depend on it without touching HttpContext.
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    string? Email { get; }

    string? DisplayName { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Permissions { get; }

    bool HasPermission(string permission);
}
