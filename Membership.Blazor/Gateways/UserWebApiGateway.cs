namespace Membership.Blazor.Gateways;
internal class UserWebApiGateway : IUserWebApiGateway
{
    readonly UserEndpointsOptions Options;
    readonly HttpClient Client;

    public UserWebApiGateway(IOptions<UserEndpointsOptions> options,
        HttpClient client)
    {
        Options = options.Value;
        Client = client;
        client.BaseAddress = new Uri(Options.WebApiBaseAddress);
    }

    public async Task<UserTokensDto> LoginAsync(UserCredentialsDto userCredentials)
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            Options.Login, userCredentials);
        return await response.Content.ReadFromJsonAsync<UserTokensDto>();
    }

    public async Task LogoutAsync(UserTokensDto userTokens)
    {
        await Client.PostAsJsonAsync(Options.Logout, userTokens);
    }

    public async Task<UserTokensDto> RefreshTokenAsync(UserTokensDto userTokens)
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            Options.RefreshToken, userTokens);
        return await response.Content.ReadFromJsonAsync<UserTokensDto>();
    }

    public async Task RegisterUserAsync(UserDto user)
    {
        await Client.PostAsJsonAsync(Options.Register, user);
    }
}
