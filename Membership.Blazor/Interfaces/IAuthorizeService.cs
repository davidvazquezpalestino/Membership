namespace Membership.Blazor.Interfaces;
public interface IAuthorizeService
{
    ExternalIDPInfo[] IDPs { get; }
    Task AuthorizeAsync(string providerId, ScopeAction action,
        string returnUri);
}
