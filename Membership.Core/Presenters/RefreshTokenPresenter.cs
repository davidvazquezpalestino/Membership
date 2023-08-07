namespace Membership.Core.Presenters;
internal class RefreshTokenPresenter : IRefreshTokenOutputPort
{
    readonly IAccessTokenService AccessTokenService;
    readonly IRefreshTokenService RefreshTokenService;

    public RefreshTokenPresenter(IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        AccessTokenService = accessTokenService;
        RefreshTokenService = refreshTokenService;
    }

    public UserTokensDto UserTokens { get; private set; }

    public async Task HandleAccessTokenAsync(string accessToken)
    {
        string accessTokenAsync =
            await AccessTokenService.RotateAccessTokenAsync(accessToken);

        string refreshToken =
            await RefreshTokenService.GetRefreshTokenForAccessTokenAsync(accessTokenAsync);

        UserTokens = new UserTokensDto(accessTokenAsync, refreshToken);
    }
}
