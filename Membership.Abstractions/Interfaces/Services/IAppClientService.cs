namespace Membership.Abstractions.Interfaces.Services;
public interface IAppClientService
{
    void ThrowIfNotExist(string clientId, string redirectUri);
}
