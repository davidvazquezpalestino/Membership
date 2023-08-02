namespace Membership.Blazor.Services;
internal class TokenService
{
    readonly IMembershipMessageLocalizer Localizer;
    readonly IOAuthStateService StateService;
    readonly IOptions<AppOptions> AppOptions;
    readonly IOAuthService OAuthService;
    readonly HttpClient Client;

    public TokenService(IMembershipMessageLocalizer localizer,
        IOAuthStateService stateService, IOptions<AppOptions> appOptions,
        IOAuthService oAuthService, HttpClient client)
    {
        Localizer = localizer;
        StateService = stateService;
        AppOptions = appOptions;
        OAuthService = oAuthService;
        Client = client;
    }

    public async Task<TokenServiceResponse> GetTokensAsync(string state, string code)
    {
        TokenServiceResponse Result;

        if (code == null || state == null)
            throw new Exception(
                Localizer[MessageKeys.MissingAuthorizeCallbackParameters]);

        var StateInfo = await StateService.GetAsync<StateInfo>(state);

        if (StateInfo == null)
            throw new Exception(Localizer[MessageKeys.InvalidStateValue]);

        var RequestBody = OAuthService.BuildTokenRequestBody(
            new TokenRequestInfo(code, AppOptions.Value.RedirectUri,
            AppOptions.Value.ClientId, StateInfo.Scope, StateInfo.CodeVerifier, null));

        var Response =
            await Client.PostAsync(AppOptions.Value.TokenEndpoint, RequestBody);

        var Tokens = await Response.Content.ReadFromJsonAsync<UserTokensDto>();
        var JwtToken = new JwtSecurityTokenHandler()
            .ReadJwtToken(Tokens.AccessToken);
        var TokenNonce = JwtToken.Claims.FirstOrDefault(c => c.Type == "nonce")?.Value;
        if (TokenNonce == null || TokenNonce != StateInfo.Nonce)
            throw new Exception(Localizer[MessageKeys.InvalidNonceValue]);

        Result = new()
        {
            Tokens = Tokens,
            ReturnUri = StateInfo.ReturnUri,
            Scope = StateInfo.Scope
        };

        await StateService.RemoveAsync(state);

        return Result;
    }
}
