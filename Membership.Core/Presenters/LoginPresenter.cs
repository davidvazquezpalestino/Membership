namespace Membership.Core.Presenters;
internal class LoginPresenter : ILoginOutputPort
{
    readonly IAccessTokenService AccessTokenService;
    readonly IRefreshTokenService RefreshTokenService;

    public LoginPresenter(IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        AccessTokenService = accessTokenService;
        RefreshTokenService = refreshTokenService;
    }

    public UserTokensDto UserTokens { get; private set; }

    public async Task HandleUserEntityAsync(UserEntity user)
    {
        string AccessToken =
            await AccessTokenService.GetNewUserAccessTokenAsync(user);
        string RefreshToken =
            await RefreshTokenService.GetRefreshTokenForAccessTokenAsync(AccessToken);
        UserTokens = new UserTokensDto(AccessToken, RefreshToken);
    }
}
