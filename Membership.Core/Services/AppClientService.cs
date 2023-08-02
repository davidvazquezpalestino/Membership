namespace Membership.Core.Services;
internal class AppClientService : IAppClientService
{
    readonly AppClientInfoOptions AppClientsInfo;

    public AppClientService(IOptions<AppClientInfoOptions> appClientsInfo)
    {
        AppClientsInfo = appClientsInfo.Value;
    }

    public void ThrowIfNotExist(string clientId, string redirectUri)
    {
        if (!AppClientsInfo.AppClients
            .Any(c => c.ClientId == clientId && c.RedirectUri == redirectUri))
            throw new UnauthorizedAccessException();
    }
}
