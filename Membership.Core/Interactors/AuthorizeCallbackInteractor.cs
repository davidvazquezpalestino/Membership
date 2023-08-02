namespace Membership.Core.Interactors;
internal class AuthorizeCallbackInteractor : IAuthorizeCallbackInputPort
{
    readonly IOAuthStateService OAuthStateService;
    readonly IIDPService IDPService;

    public AuthorizeCallbackInteractor(IOAuthStateService oAuthStateService,
        IIDPService iDPService)
    {
        OAuthStateService = oAuthStateService;
        IDPService = iDPService;
    }

    public async Task<string> HandleCallback(string state, string code)
    {
        var StateInfo = await OAuthStateService.GetAsync<StateInfo>(state);
        if (StateInfo == null) throw new MissingCallbackStateParameterException();

        var Tokens = await IDPService.GetTokensAsync(
            StateInfo.ProviderId, code, StateInfo.CodeVerifier, StateInfo.Nonce);

        if (Tokens == null) throw new UnableToGetIDPTokensException();
        StateInfo.Tokens = Tokens;

        string RedirectUri = string.Format("{0}?state={1}&code={2}",
            StateInfo.AppClientStateInfo.RedirectUri,
            StateInfo.AppClientStateInfo.State, state);
        return RedirectUri;
    }
}
