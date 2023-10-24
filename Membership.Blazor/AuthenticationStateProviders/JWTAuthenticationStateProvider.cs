namespace Membership.Blazor.AuthenticationStateProviders;
internal class JwtAuthenticationStateProvider : AuthenticationStateProvider, IAuthenticationStateProvider
{
    readonly IUserWebApiGateway UserWebApiGateway;
    readonly ITokensRepository TokensRepository;
    private readonly ILogger<JwtAuthenticationStateProvider> Logger;

    public JwtAuthenticationStateProvider(IUserWebApiGateway userWebApiGateway, 
        ITokensRepository tokensRepository, ILogger<JwtAuthenticationStateProvider> logger)
    {
        UserWebApiGateway = userWebApiGateway;
        TokensRepository = tokensRepository;
        Logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity identity = new ClaimsIdentity();

        UserTokensDto storedTokens = await GetUserTokensAsync();
        if (storedTokens != null)
        {
            JwtSecurityToken token = new JwtSecurityTokenHandler()
                .ReadJwtToken(storedTokens.AccessToken);

            identity = new ClaimsIdentity(token.Claims, "Bearer");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<UserTokensDto> GetUserTokensAsync()
    {
        UserTokensDto storedTokens = await TokensRepository.GetTokensAsync();
        if (storedTokens != null)
        {
            JwtSecurityToken token = new JwtSecurityTokenHandler()
                .ReadJwtToken(storedTokens.AccessToken);

            if (token.ValidTo <= DateTime.UtcNow)
            {
                try
                {
                    UserTokensDto newTokens = await UserWebApiGateway.RefreshTokenAsync(storedTokens);
                    await LoginAsync(newTokens);
                    storedTokens = newTokens;

                    Logger.LogInformation("Access token actualizado");
                }
                catch (Exception ex)
                {
                    storedTokens = default;
                    Logger.LogError(ex.Message);
                    await LogoutAsync();
                }
            }
        }
        return storedTokens;
    }

    public async Task LoginAsync(UserTokensDto userTokensDto)
    {
        await TokensRepository.SaveTokensAsync(userTokensDto);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await TokensRepository.RemoveTokensAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

