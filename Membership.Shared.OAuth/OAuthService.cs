﻿namespace Membership.Shared.OAuth;
internal class OAuthService : IOAuthService
{
    public string GetState() =>
        GetRandomString(32);
    public string GetNonce() =>
        GetRandomString(12);

    public string GetCodeVerifier()
    {
        // https://www.oauth.com/oauth2-servers/pkce/authorization-request/
        const string possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        
        StringBuilder stringBuilder = new StringBuilder();
        int maxIndex = possibleChars.Length;
        Random randomGenerator = new Random();
        // Code Verifier debe tener una longitud de 43 a 128.
        int length = randomGenerator.Next(43, 129);
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(possibleChars[randomGenerator.Next(maxIndex)]);
        }
        return stringBuilder.ToString();
    }

    public string GetHash256CodeChallenge(string codeVerifier)
    {
        byte[] challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return Base64UrlEncoder.Encode(challengeBytes);
    }

    public string BuildAuthorizeRequestUri(AuthorizeRequestInfo info)
    {
        StringBuilder stringBuilder = new StringBuilder($"{info.AuthorizeEndpoint}?");
        stringBuilder.Append($"response_type=code&");
        stringBuilder.Append($"client_id={info.ClientId}&");
        stringBuilder.Append($"redirect_uri={info.RedirectUri}&");
        stringBuilder.Append($"state={info.State}&");
        stringBuilder.Append($"scope={info.Scope}&");
        stringBuilder.Append($"code_challenge={info.CodeChallenge}&");
        stringBuilder.Append($"code_challenge_method={info.CodeChallengeMethod}&");
        stringBuilder.Append($"nonce={info.Nonce}");
        
        return stringBuilder.ToString();
    }

    public FormUrlEncodedContent BuildTokenRequestBody(TokenRequestInfo info)
    {
        Dictionary<string, string> bodyData =
            new()
            {
                {"grant_type", "authorization_code" },
                {"code", info.Code },
                {"redirect_uri", info.RedirectUri },
                {"client_id", info.ClientId},
                {"scope", info.Scope},
                {"code_verifier", info.CodeVerifier }
            };

        if (!string.IsNullOrWhiteSpace(info.ClientSecret))
        {
            bodyData.Add("client_secret", info.ClientSecret);
        }

        return new FormUrlEncodedContent(bodyData);
    }

    public TokenRequestInfo GetTokenRequestInfoFromRequestBody(Dictionary<string, string> requestBody)
    {
        requestBody.TryGetValue("code", out string code);
        requestBody.TryGetValue("redirect_uri", out string redirectUri);
        requestBody.TryGetValue("client_id", out string clientId);
        requestBody.TryGetValue("scope", out string scope);
        requestBody.TryGetValue("code_verifier", out string codeVerifier);
        requestBody.TryGetValue("client_secret", out string clientSecret);

        return new TokenRequestInfo(code, redirectUri, clientId,
            scope, codeVerifier, clientSecret);

    }

    #region Helpers
    static string GetRandomString(int length)
    {
        // 3 bytes generan 4 caracteres base64
        // 75 bytes generan 100 caracteres

        double numBytes = Math.Ceiling(3 * length / 4d);
        byte[] buffer = new byte[(int)numBytes];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetNonZeroBytes(buffer);

        // Requiere paquete Microsoft.IdentityModel.Tokens
        return Base64UrlEncoder.Encode(buffer);
    }
    #endregion

}
