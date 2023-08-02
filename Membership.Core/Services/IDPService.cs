namespace Membership.Core.Services;
internal class IDPService : IIDPService
{
    readonly HttpClient Client;
    readonly IOAuthService OAuthService;
    readonly IDPClientInfoOptions ClientIDPInfoOptions;
    readonly ILogger<IDPService> Logger;

    public IDPService(IHttpClientFactory httpClientFactory,
        IOAuthService oAuthService,
        IOptions<IDPClientInfoOptions> clientIDPInfoOptions,
        ILogger<IDPService> logger)
    {
        Client = httpClientFactory.CreateClient();
        OAuthService = oAuthService;
        ClientIDPInfoOptions = clientIDPInfoOptions.Value;
        Logger = logger;
    }

    public Task<string> GetAuthorizeRequestUri(
        string providerId, string state, string codeVerifier, string nonce)
    {
        string Result = null;

        var Info = ClientIDPInfoOptions.IDPClients.FirstOrDefault(
            p => p.ProviderId == providerId);

        if (Info != null)
        {
            string CodeChallenge;
            string CodeChallengeMethod;
            if (Info.SupportsS256CodeChallengeMethod)
            {
                CodeChallenge = OAuthService.GetHash256CodeChallenge(codeVerifier);
                CodeChallengeMethod = OAuthService.CodeChallengeMethodSha256;
            }
            else
            {
                CodeChallenge = codeVerifier;
                CodeChallengeMethod = OAuthService.CodeChallengeMethodPlain;
            }

            AuthorizeRequestInfo RequestInfo = new(Info.AuthorizeEndpoint,
                Info.ClientId, Info.RedirectUri, state, Info.Scope,
                CodeChallenge,
                CodeChallengeMethod, nonce);

            Result = OAuthService.BuildAuthorizeRequestUri(RequestInfo);
        }
        return Task.FromResult(Result);
    }

    public async Task<IDPTokens> GetTokensAsync(string providerId,
        string authorizationCode,
        string codeVerifier, string nonce)
    {
        IDPTokens Tokens = null;
        var Info = ClientIDPInfoOptions.IDPClients.FirstOrDefault(
            p => p.ProviderId == providerId);

        var RequestBody = OAuthService.BuildTokenRequestBody(
            new TokenRequestInfo(
            authorizationCode, Info.RedirectUri, Info.ClientId, Info.Scope,
            codeVerifier, Info.ClientSecret));

        var Response = await Client.PostAsync(Info.TokenEndpoint, RequestBody);
        var JsonContentResponse =
            await Response.Content.ReadFromJsonAsync<JsonElement>();

        if (Response.IsSuccessStatusCode)
        {
            if (JsonContentResponse.TryGetProperty("id_token",
                out JsonElement idTokenJson))
            {
                string IdTokenToVerify = idTokenJson.ToString();
                // Requiere el paquete NuGet: System.IdentityModel.Tokens.Jwt
                var Handler = new JwtSecurityTokenHandler();
                var JwtToken = Handler.ReadJwtToken(IdTokenToVerify);
                var IdTokenNonce = JwtToken.Claims.FirstOrDefault(
                    c => c.Type == "nonce")?.Value;
                if (IdTokenNonce != null && IdTokenNonce == nonce)
                {
                    Tokens = new()
                    {
                        IdToken = IdTokenToVerify
                    };

                    if (JsonContentResponse.TryGetProperty("access_token",
                        out JsonElement accessTokenJson))
                        Tokens.AccessToken = accessTokenJson.ToString();
                }
            }
        }
        else
        {
            Logger.LogError("{content}", JsonContentResponse.GetRawText());
        }
        return Tokens;
    }
}

