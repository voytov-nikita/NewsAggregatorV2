using AuthService.DAL.Abstractions.Stores;
using AuthService.DAL.PostgreSql.Entities;
using AuthService.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.PostgreSql.Stores;

public class UsersStore(
    UserManager<ApplicationUser> userManager,
    IPermissionsStore permissionsStore,
    AuthServiceDbContext context) : IUsersStore
{
    public async Task<CreateUserResultModel> CreateAsync(CreateUserModel model, string role)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            UserName = model.UserName,
            DisplayName = model.DisplayName,
            CreatedAt = DateTime.UtcNow,
        };

        IdentityResult result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return new CreateUserResultModel
            {
                Succeeded = false,
                Errors = result.Errors.Select(_ => _.Description).ToArray(),
            };
        }

        await userManager.AddToRoleAsync(user, role);

        return new CreateUserResultModel
        {
            Succeeded = true,
            User = await ToModelAsync(user),
        };
    }

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(email);

        return user is null ? null : await ToModelAsync(user);
    }

    public async Task<UserModel?> FindByIdAsync(Guid id)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id.ToString());

        return user is null ? null : await ToModelAsync(user);
    }

    public async Task<bool> VerifyPasswordAsync(Guid userId, string password)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());

        return user is not null && await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> IsLockedOutAsync(Guid userId)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());

        return user is not null && await userManager.IsLockedOutAsync(user);
    }

    public async Task RegisterFailedAttemptAsync(Guid userId)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user is not null)
        {
            await userManager.AccessFailedAsync(user);
        }
    }

    public async Task ResetFailedAttemptsAsync(Guid userId)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user is not null)
        {
            await userManager.ResetAccessFailedCountAsync(user);
        }
    }

    public async Task SetLastLoginAsync(Guid userId, DateTime loginAt) =>
        await context.Users
            .Where(_ => _.Id == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(_ => _.LastLoginAt, loginAt));

    private async Task<UserModel> ToModelAsync(ApplicationUser user)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);
        IReadOnlyCollection<string> permissions = await permissionsStore.GetForRolesAsync(roles.ToArray());

        return new UserModel
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.ToArray(),
            Permissions = permissions,
        };
    }
}
