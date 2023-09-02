namespace Membership.Core.Interactors;
internal class AuthorizeCallbackInteractor : IAuthorizeCallbackInputPort
{
    readonly IOAuthStateService OAuthStateService;
    readonly IIDPService IdpService;

    public AuthorizeCallbackInteractor(IOAuthStateService oAuthStateService,
        IIDPService iDpService)
    {
        OAuthStateService = oAuthStateService;
        IdpService = iDpService;
    }

    public async Task<string> HandleCallback(string state, string code)
    {
        StateInfo stateInfo = await OAuthStateService.GetAsync<StateInfo>(state);
        if (stateInfo == null)
        {
            throw new MissingCallbackStateParameterException();
        }

        IDPTokens tokens = await IdpService.GetTokensAsync(
            stateInfo.ProviderId, code, stateInfo.CodeVerifier, stateInfo.Nonce);

        if (tokens == null)
        {
            throw new UnableToGetIDPTokensException();
        }

        stateInfo.Tokens = tokens;

        string redirectUri = string.Format("{0}?state={1}&code={2}",
            stateInfo.AppClientStateInfo.RedirectUri,
            stateInfo.AppClientStateInfo.State, state);
        return redirectUri;
    }
}
