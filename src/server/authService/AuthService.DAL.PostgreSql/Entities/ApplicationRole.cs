using Microsoft.AspNetCore.Identity;

namespace AuthService.DAL.PostgreSql.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
