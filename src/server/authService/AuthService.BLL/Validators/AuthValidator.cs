using System.Text.RegularExpressions;

using AuthService.BLL.Abstractions.Validators;
using AuthService.Models.Auth;

namespace AuthService.BLL.Validators;

public partial class AuthValidator : IAuthValidator
{
    private const int MaxEmailLength = 256;
    private const int MaxUserNameLength = 256;

    public void Validate(RegisterModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ValidateEmail(model.Email);

        if (string.IsNullOrWhiteSpace(model.UserName))
        {
            throw new ArgumentException("User name is required.", nameof(model));
        }

        if (model.UserName.Length > MaxUserNameLength)
        {
            throw new ArgumentException($"User name must be at most {MaxUserNameLength} characters.", nameof(model));
        }

        // Length and complexity rules live in Identity's PasswordOptions; only emptiness is a shape problem.
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            throw new ArgumentException("Password is required.", nameof(model));
        }
    }

    public void Validate(LoginModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ValidateEmail(model.Email);

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            throw new ArgumentException("Password is required.", nameof(model));
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (email.Length > MaxEmailLength)
        {
            throw new ArgumentException($"Email must be at most {MaxEmailLength} characters.", nameof(email));
        }

        if (!EmailRegex().IsMatch(email))
        {
            throw new ArgumentException("Email is not a valid address.", nameof(email));
        }
    }
}
