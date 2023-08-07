namespace Membership.Core.Interactors;
internal class AuthorizeCallbackInteractor : IAuthorizeCallbackInputPort
{
    readonly IOAuthStateService OAuthStateService;
    readonly IIDPService IDPService;

    public AuthorizeCallbackInteractor(IOAuthStateService oAuthStateService,
        IIDPService idpService)
    {
        OAuthStateService = oAuthStateService;
        IDPService = idpService;
    }

    public async Task<string> HandleCallback(string state, string code)
    {
        StateInfo stateInfo = await OAuthStateService.GetAsync<StateInfo>(state);
        if (stateInfo == null)
        {
            throw new MissingCallbackStateParameterException();
        }

        IDPTokens tokens = await IDPService.GetTokensAsync(
            stateInfo.ProviderId, code, stateInfo.CodeVerifier, stateInfo.Nonce);

        if (tokens == null)
        {
            throw new UnableToGetIDPTokensException();
        }

        stateInfo.Tokens = tokens;

        string redirectUri =
            $"{stateInfo.AppClientStateInfo.RedirectUri}?state={stateInfo.AppClientStateInfo.State}&code={state}";
        return redirectUri;
    }
}
