namespace BlazorWasmApp.Gateways;

public class WebApiClient
{
    readonly HttpClient Client;
    public WebApiClient(HttpClient client) => Client = client;

    public async Task<string> GetAnonymousMessage() =>
        await Client.GetFromJsonAsync<string>("anonymous");
    public async Task<string> GetAuthorizedUserMessage() =>
        await Client.GetFromJsonAsync<string>("authorizeduser");
}
