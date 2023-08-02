namespace Membership.Abstractions.Interfaces.RefreshToken;
public interface IRefreshTokenOutputPort
{
    UserTokensDto UserTokens { get; }
    Task HandleAccessTokenAsync(string accessToken);
}
