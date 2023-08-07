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
        if (code == null || state == null)
        {
            throw new Exception(
                Localizer[MessageKeys.MissingAuthorizeCallbackParameters]);
        }

        StateInfo stateInfo = await StateService.GetAsync<StateInfo>(state);

        if (stateInfo == null)
        {
            throw new Exception(Localizer[MessageKeys.InvalidStateValue]);
        }

        FormUrlEncodedContent requestBody = OAuthService.BuildTokenRequestBody(
            new TokenRequestInfo(code, AppOptions.Value.RedirectUri,
            AppOptions.Value.ClientId, stateInfo.Scope, stateInfo.CodeVerifier, null));

        HttpResponseMessage response =
            await Client.PostAsync(AppOptions.Value.TokenEndpoint, requestBody);

        UserTokensDto tokens = await response.Content.ReadFromJsonAsync<UserTokensDto>();
        JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokens.AccessToken);
        
        string tokenNonce = jwtToken.Claims.FirstOrDefault(c => c.Type == "nonce")?.Value;
        if (tokenNonce == null || tokenNonce != stateInfo.Nonce)
        {
            throw new Exception(Localizer[MessageKeys.InvalidNonceValue]);
        }

        TokenServiceResponse result = new()
        {
            Tokens = tokens,
            ReturnUri = stateInfo.ReturnUri,
            Scope = stateInfo.Scope
        };

        await StateService.RemoveAsync(state);

        return result;
    }
}
