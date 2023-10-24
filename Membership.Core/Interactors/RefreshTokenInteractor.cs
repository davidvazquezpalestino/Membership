namespace Membership.Core.Interactors;
internal class RefreshTokenInteractor : IRefreshTokenInputPort
{
    readonly IRefreshTokenService RefreshTokenService;
    readonly IRefreshTokenOutputPort OutputPort;

    public RefreshTokenInteractor(IRefreshTokenService refreshTokenService,
        IRefreshTokenOutputPort outputPort)
    {
        RefreshTokenService = refreshTokenService;
        OutputPort = outputPort;
    }

    public async Task RefreshTokenAsync(UserTokensDto userTokens)
    {
        await RefreshTokenService.ThrowIfUnableToRotateRefreshTokenAsync(userTokens.RefreshToken, userTokens.AccessToken);

        await OutputPort.HandleAccessTokenAsync(userTokens.AccessToken);
    }
}
