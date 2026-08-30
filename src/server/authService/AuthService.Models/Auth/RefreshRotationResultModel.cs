namespace AuthService.Models.Auth;

public class RefreshRotationResultModel
{
    public bool Succeeded { get; set; }

    public RefreshErrorCode Error { get; set; }

    public Guid UserId { get; set; }

    public RawRefreshTokenModel? Token { get; set; }

    public static RefreshRotationResultModel Success(Guid userId, RawRefreshTokenModel token) => new()
    {
        Succeeded = true,
        UserId = userId,
        Token = token,
    };

    public static RefreshRotationResultModel Failure(RefreshErrorCode error) => new()
    {
        Succeeded = false,
        Error = error,
    };
}
