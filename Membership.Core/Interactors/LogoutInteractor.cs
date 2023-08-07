namespace Membership.Core.Interactors;
internal class LogoutInteractor : ILogoutInputPort
{
    readonly IRefreshTokenService RefreshTokenService;
    public LogoutInteractor(IRefreshTokenService refreshTokenService) => 
        RefreshTokenService = refreshTokenService;

    public async Task LogoutAsync(UserTokensDto userTokens) => 
        await RefreshTokenService.DeleteRefreshTokenAsync(userTokens.RefreshToken);
}
