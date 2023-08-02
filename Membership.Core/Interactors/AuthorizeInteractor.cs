namespace Membership.Core.Interactors;
internal class AuthorizeInteractor : IAuthorizeInputPort
{
    readonly IAppClientService AppClientService;
    readonly IOAuthService OAuthService;
    readonly IIDPService IDPService;
    readonly IOAuthStateService OAuthStateService;

    public AuthorizeInteractor(IAppClientService appClientService,
        IOAuthService oauthService, IIDPService iDPService,
        IOAuthStateService oAuthStateService)
    {
        AppClientService = appClientService;
        OAuthService = oauthService;
        IDPService = iDPService;
        OAuthStateService = oAuthStateService;
    }

    public async Task<string> GetAuthorizeRequestRedirectUri(
        AppClientAuthorizeRequestInfo requestInfo)
    {
        AppClientService.ThrowIfNotExist(requestInfo.ClientId,
            requestInfo.RedirectUri);

        string State = OAuthService.GetState();
        var RequestState = new StateInfo
        {
            CodeVerifier = OAuthService.GetCodeVerifier(),
            Nonce = OAuthService.GetNonce(),
            ProviderId = requestInfo.Scope.Substring(
                requestInfo.Scope.IndexOf("_") + 1),
            AppClientStateInfo = requestInfo
        };

        string RequestUri = await IDPService.GetAuthorizeRequestUri(
            RequestState.ProviderId, State, RequestState.CodeVerifier,
            RequestState.Nonce);

        if (RequestUri == null) throw new UnauthorizedAccessException();
        await OAuthStateService.SetAsync(State, RequestState);
        return RequestUri;
    }
}
