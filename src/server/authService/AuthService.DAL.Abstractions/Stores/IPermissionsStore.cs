namespace AuthService.DAL.Abstractions.Stores;

public interface IPermissionsStore
{
    Task<IReadOnlyCollection<string>> GetAllNamesAsync();

    Task<IReadOnlyCollection<string>> GetForRolesAsync(IReadOnlyCollection<string> roleNames);
}
