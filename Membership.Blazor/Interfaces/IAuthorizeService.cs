namespace Membership.Blazor.Interfaces;
public interface IAuthorizeService
{
    ExternalIdpInfo[] IDPs { get; }
    Task AuthorizeAsync(string providerId, ScopeAction action,
        string returnUri);
}
