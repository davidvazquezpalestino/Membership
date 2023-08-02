namespace Membership.Abstractions.Interfaces.Authorize;
public interface IAuthorizeInputPort
{
    Task<string> GetAuthorizeRequestRedirectUri(
        AppClientAuthorizeRequestInfo requestInfo);
}
