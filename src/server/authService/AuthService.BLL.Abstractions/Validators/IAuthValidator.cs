using AuthService.Models.Auth;

namespace AuthService.BLL.Abstractions.Validators;

/// <summary>
/// Shape-level validation only (empty password, malformed email). Business failures such as a
/// wrong password or a duplicate email come back as <see cref="AuthErrorCode"/> values instead.
/// </summary>
public interface IAuthValidator
{
    void Validate(RegisterModel model);

    void Validate(LoginModel model);
}
