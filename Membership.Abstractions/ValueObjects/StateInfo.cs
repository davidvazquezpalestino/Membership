namespace Membership.Abstractions.ValueObjects;
public class StateInfo
{
    public string CodeVerifier { get; set; }
    public string Nonce { get; set; }
    public string ProviderId { get; set; }
    public AppClientAuthorizeRequestInfo AppClientStateInfo { get; set; }
    public IDPTokens Tokens { get; set; }
}
