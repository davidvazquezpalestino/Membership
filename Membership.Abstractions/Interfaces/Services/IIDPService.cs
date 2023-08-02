namespace Membership.Abstractions.Interfaces.Services;
public interface IIDPService
{
    Task<string> GetAuthorizeRequestUri(string providerId, string state,
        string codeVerifier, string nonce);

    Task<IDPTokens> GetTokensAsync(string providerId,
        string authorizationCode, string codeVerifier, string nonce);
}
