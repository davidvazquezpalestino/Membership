namespace Membership.Core.Interactors;
internal class AuthorizeInteractor : IAuthorizeInputPort
{
    readonly IAppClientService AppClientService;
    readonly IOAuthService OAuthService;
    readonly IIDPService IdpService;
    readonly IOAuthStateService OAuthStateService;

    public AuthorizeInteractor(IAppClientService appClientService,
        IOAuthService oauthService, IIDPService iDpService,
        IOAuthStateService oAuthStateService)
    {
        AppClientService = appClientService;
        OAuthService = oauthService;
        IdpService = iDpService;
        OAuthStateService = oAuthStateService;
    }

    public async Task<string> GetAuthorizeRequestRedirectUri(
        AppClientAuthorizeRequestInfo requestInfo)
    {
        AppClientService.ThrowIfNotExist(requestInfo.ClientId,
            requestInfo.RedirectUri);

        string state = OAuthService.GetState();
        StateInfo requestState = new StateInfo
        {
            CodeVerifier = OAuthService.GetCodeVerifier(),
            Nonce = OAuthService.GetNonce(),
            ProviderId = requestInfo.Scope.Substring(
                requestInfo.Scope.IndexOf("_", StringComparison.Ordinal) + 1),
            AppClientStateInfo = requestInfo
        };

        string requestUri = await IdpService.GetAuthorizeRequestUri(
            requestState.ProviderId, state, requestState.CodeVerifier,
            requestState.Nonce);

        if (requestUri == null)
        {
            throw new UnauthorizedAccessException();
        }

        await OAuthStateService.SetAsync(state, requestState);
        return requestUri;
    }
}
