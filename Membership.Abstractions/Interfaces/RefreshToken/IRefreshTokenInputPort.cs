namespace Membership.Abstractions.Interfaces.RefreshToken;
public interface IRefreshTokenInputPort
{
    Task RefreshTokenAsync(UserTokensDto userTokens);
}
