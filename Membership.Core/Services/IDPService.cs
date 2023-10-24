namespace Membership.Core.Services;
internal class IdpService : IIDPService
{
    readonly HttpClient Client;
    readonly IOAuthService OAuthService;
    readonly IDPClientInfoOptions ClientIdpInfoOptions;
    readonly ILogger<IdpService> Logger;

    public IdpService(IHttpClientFactory httpClientFactory,
        IOAuthService oAuthService,
        IOptions<IDPClientInfoOptions> clientIdpInfoOptions,
        ILogger<IdpService> logger)
    {
        Client = httpClientFactory.CreateClient();
        OAuthService = oAuthService;
        ClientIdpInfoOptions = clientIdpInfoOptions.Value;
        Logger = logger;
    }

    public Task<string> GetAuthorizeRequestUri(string providerId, string state, string codeVerifier, string nonce)
    {
        string result = null;
        IDPClientInfo info = ClientIdpInfoOptions.IDPClients.FirstOrDefault(clientInfo => clientInfo.ProviderId == providerId);

        if (info != null)
        {
            string codeChallenge;
            string codeChallengeMethod;
            if (info.SupportsS256CodeChallengeMethod)
            {
                codeChallenge = OAuthService.GetHash256CodeChallenge(codeVerifier);
                codeChallengeMethod = OAuthService.CodeChallengeMethodSha256;
            }
            else
            {
                codeChallenge = codeVerifier;
                codeChallengeMethod = OAuthService.CodeChallengeMethodPlain;
            }

            AuthorizeRequestInfo requestInfo = new(info.AuthorizeEndpoint,
                info.ClientId, info.RedirectUri, state, info.Scope,
                codeChallenge, codeChallengeMethod, nonce);

            result = OAuthService.BuildAuthorizeRequestUri(requestInfo);
        }
        return Task.FromResult(result);
    }

    public async Task<IDPTokens> GetTokensAsync(string providerId,
        string authorizationCode,
        string codeVerifier, string nonce)
    {
        IDPTokens tokens = null;
        IDPClientInfo info = ClientIdpInfoOptions.IDPClients.FirstOrDefault(clientInfo => clientInfo.ProviderId == providerId);

        FormUrlEncodedContent requestBody = OAuthService.BuildTokenRequestBody(
            new TokenRequestInfo(authorizationCode, info.RedirectUri, info.ClientId, info.Scope, codeVerifier, info.ClientSecret));

        HttpResponseMessage response = await Client.PostAsync(info.TokenEndpoint, requestBody);
        JsonElement jsonContentResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (response.IsSuccessStatusCode)
        {
            if (jsonContentResponse.TryGetProperty("id_token",
                out JsonElement idTokenJson))
            {
                string idTokenToVerify = idTokenJson.ToString();
                // Requiere el paquete NuGet: System.IdentityModel.Tokens.Jwt
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(idTokenToVerify);

                string idTokenNonce = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nonce")?.Value;
                if (idTokenNonce != null && idTokenNonce == nonce)
                {
                    tokens = new()
                    {
                        IdToken = idTokenToVerify
                    };

                    if (jsonContentResponse.TryGetProperty("access_token", out JsonElement accessTokenJson))
                    {
                        tokens.AccessToken = accessTokenJson.ToString();
                    }
                }
            }
        }
        else
        {
            Logger.LogError("{content}", jsonContentResponse.GetRawText());
        }
        return tokens;
    }
}

