namespace Membership.Blazor.Interfaces;
public interface IAuthorizeService
{
    ExternalIDPInfo[] IDPInfos { get; }
    Task AuthorizeAsync(string providerId, ScopeAction action, string returnUri);
}
